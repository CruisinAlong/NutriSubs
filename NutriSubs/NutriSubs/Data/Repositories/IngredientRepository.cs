using NutriSubs.Data.Entities;
using NutriSubs.Services;
using SQLite;

namespace NutriSubs.Data.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly SQLiteAsyncConnection _connection;

        public IngredientRepository(SubstitutionDatabase database)
        {
            _connection = database.Connection;
        }

        public Task<IngredientEntity?> GetByIdAsync(int id)
        {
            return _connection.Table<IngredientEntity>()
                .Where(i => i.Id == id && i.IsActive)
                .FirstOrDefaultAsync()!;
        }

        public Task<IngredientEntity?> GetByKeyAsync(string key)
        {
            return _connection.Table<IngredientEntity>()
                .Where(i => i.Key == key)
                .FirstOrDefaultAsync()!;
        }

        public async Task<IngredientEntity?> FindByNameOrAliasAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return null;

            var normalized = searchTerm.Trim();

            // 1. Exact canonical-name match (case-insensitive).
            var direct = await _connection.QueryAsync<IngredientEntity>(
                "SELECT * FROM Ingredients WHERE IsActive = 1 AND Name = ? COLLATE NOCASE LIMIT 1",
                normalized);

            if (direct.Count > 0)
                return direct[0];

            // 2. Exact alias match (case-insensitive).
            var byAlias = await _connection.QueryAsync<IngredientEntity>(
                @"SELECT i.* FROM Ingredients i
                  INNER JOIN IngredientAliases a ON a.IngredientId = i.Id
                  WHERE i.IsActive = 1 AND a.Alias = ? COLLATE NOCASE LIMIT 1",
                normalized);

            return byAlias.Count > 0 ? byAlias[0] : null;
        }

        public async Task<IngredientEntity?> FindMatchInTextAsync(string ingredientText)
        {
            if (string.IsNullOrWhiteSpace(ingredientText))
                return null;

            // Parse the line into structured fields first (quantity/unit/name) instead of
            // searching the whole original string, e.g. "3 tablespoons calamansi" -> "calamansi".
            var parsed = IngredientParsingService.Parse(ingredientText);

            if (parsed.ParseStatus == Models.IngredientParseStatus.Failed || string.IsNullOrWhiteSpace(parsed.IngredientName))
                return null;

            // 1 & 2: exact canonical-name / exact alias match against the normalized name.
            var exactMatch = await FindByNameOrAliasAsync(parsed.IngredientName);
            if (exactMatch is not null)
                return exactMatch;

            // 3. Case-insensitive normalized match is already covered by the COLLATE NOCASE
            // comparisons above since IngredientName is already normalized (trimmed,
            // singularized, descriptors/preparation stripped).

            // 4. Carefully constrained partial match: only when the normalized ingredient
            // name is itself a whole phrase contained within (or containing) a canonical
            // name/alias, never a loose substring of the raw original text. This avoids
            // "coconut milk" incorrectly matching "coconut cream".
            return await FindConstrainedPartialMatchAsync(parsed.IngredientName);
        }

        private async Task<IngredientEntity?> FindConstrainedPartialMatchAsync(string normalizedName)
        {
            var candidates = await _connection.QueryAsync<IngredientNameMatch>(
                @"SELECT i.Id as IngredientId, i.Name as MatchedText FROM Ingredients i WHERE i.IsActive = 1
                  UNION ALL
                  SELECT a.IngredientId as IngredientId, a.Alias as MatchedText FROM IngredientAliases a
                  INNER JOIN Ingredients i ON i.Id = a.IngredientId WHERE i.IsActive = 1");

            var normalizedLower = normalizedName.ToLowerInvariant();
            var normalizedWords = normalizedLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            IngredientNameMatch? best = null;

            foreach (var candidate in candidates)
            {
                if (string.IsNullOrWhiteSpace(candidate.MatchedText))
                    continue;

                var candidateLower = candidate.MatchedText.ToLowerInvariant();
                var candidateWords = candidateLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (!IngredientNameMatcher.IsConstrainedPartialMatch(normalizedWords, candidateWords))
                    continue;

                if (best is null || candidate.MatchedText.Length > best.MatchedText.Length)
                    best = candidate;
            }

            return best is null ? null : await GetByIdAsync(best.IngredientId);
        }

        private class IngredientNameMatch
        {
            public int IngredientId { get; set; }
            public string MatchedText { get; set; } = string.Empty;
        }

        public Task<List<IngredientAliasEntity>> GetAliasesAsync(int ingredientId)
        {
            return _connection.Table<IngredientAliasEntity>()
                .Where(a => a.IngredientId == ingredientId)
                .ToListAsync();
        }

        public Task<List<IngredientEntity>> GetAllActiveAsync()
        {
            return _connection.Table<IngredientEntity>()
                .Where(i => i.IsActive)
                .ToListAsync();
        }

        public async Task<int> UpsertAsync(IngredientEntity ingredient)
        {
            var existing = await GetByKeyAsync(ingredient.Key);

            if (existing is null)
            {
                ingredient.CreatedAt = DateTime.UtcNow;
                await _connection.InsertAsync(ingredient);
                return ingredient.Id;
            }

            ingredient.Id = existing.Id;
            ingredient.CreatedAt = existing.CreatedAt;
            ingredient.UpdatedAt = DateTime.UtcNow;
            await _connection.UpdateAsync(ingredient);
            return existing.Id;
        }

        public async Task UpsertAliasAsync(IngredientAliasEntity alias)
        {
            var existing = await _connection.Table<IngredientAliasEntity>()
                .Where(a => a.IngredientId == alias.IngredientId && a.Alias == alias.Alias)
                .FirstOrDefaultAsync();

            if (existing is null)
            {
                await _connection.InsertAsync(alias);
            }
        }
    }
}
