using NutriSubs.Data.Entities;
using SQLite;

namespace NutriSubs.Data.Repositories
{
    public class SubstitutionRepository : ISubstitutionRepository
    {
        private readonly SQLiteAsyncConnection _connection;

        public SubstitutionRepository(SubstitutionDatabase database)
        {
            _connection = database.Connection;
        }

        public async Task<List<SubstitutionWithDetails>> GetForIngredientAsync(int ingredientId)
        {
            var substitutions = await _connection.Table<SubstitutionEntity>()
                .Where(s => s.OriginalIngredientId == ingredientId && s.IsActive)
                .ToListAsync();

            var result = new List<SubstitutionWithDetails>(substitutions.Count);

            foreach (var substitution in substitutions)
            {
                var components = await _connection.Table<SubstitutionComponentEntity>()
                    .Where(c => c.SubstitutionId == substitution.Id)
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync();

                var useCases = await _connection.Table<SubstitutionUseCaseEntity>()
                    .Where(u => u.SubstitutionId == substitution.Id)
                    .ToListAsync();

                SourceEntity? source = null;
                if (substitution.SourceId.HasValue)
                {
                    source = await _connection.Table<SourceEntity>()
                        .Where(s => s.Id == substitution.SourceId.Value)
                        .FirstOrDefaultAsync();
                }

                result.Add(new SubstitutionWithDetails
                {
                    Substitution = substitution,
                    Components = components,
                    UseCases = useCases.Select(u => u.UseCase).ToList(),
                    Source = source
                });
            }

            return result;
        }

        public async Task<int> UpsertSubstitutionAsync(SubstitutionEntity substitution)
        {
            SubstitutionEntity? existing = null;

            if (!string.IsNullOrEmpty(substitution.SeedKey))
            {
                existing = await _connection.Table<SubstitutionEntity>()
                    .Where(s => s.SeedKey == substitution.SeedKey)
                    .FirstOrDefaultAsync();
            }

            if (existing is null)
            {
                substitution.CreatedAt = DateTime.UtcNow;
                await _connection.InsertAsync(substitution);
                return substitution.Id;
            }

            substitution.Id = existing.Id;
            substitution.CreatedAt = existing.CreatedAt;
            substitution.UpdatedAt = DateTime.UtcNow;
            await _connection.UpdateAsync(substitution);
            return existing.Id;
        }

        public async Task ReplaceComponentsAsync(int substitutionId, IEnumerable<SubstitutionComponentEntity> components)
        {
            // Seeding is run at startup, not on a hot UI path, so delete+reinsert per
            // substitution is simple and safe; the data volume (MVP scale) makes this cheap.
            await _connection.RunInTransactionAsync(conn =>
            {
                conn.Execute("DELETE FROM SubstitutionComponents WHERE SubstitutionId = ?", substitutionId);

                foreach (var component in components)
                {
                    component.SubstitutionId = substitutionId;
                    conn.Insert(component);
                }
            });
        }

        public async Task ReplaceUseCasesAsync(int substitutionId, IEnumerable<SubstitutionUseCase> useCases)
        {
            await _connection.RunInTransactionAsync(conn =>
            {
                conn.Execute("DELETE FROM SubstitutionUseCases WHERE SubstitutionId = ?", substitutionId);

                foreach (var useCase in useCases)
                {
                    conn.Insert(new SubstitutionUseCaseEntity
                    {
                        SubstitutionId = substitutionId,
                        UseCase = useCase
                    });
                }
            });
        }

        public async Task<int> UpsertSourceAsync(SourceEntity source)
        {
            SourceEntity? existing = null;

            if (!string.IsNullOrEmpty(source.SeedKey))
            {
                existing = await _connection.Table<SourceEntity>()
                    .Where(s => s.SeedKey == source.SeedKey)
                    .FirstOrDefaultAsync();
            }

            if (existing is null)
            {
                await _connection.InsertAsync(source);
                return source.Id;
            }

            source.Id = existing.Id;
            await _connection.UpdateAsync(source);
            return existing.Id;
        }
    }
}
