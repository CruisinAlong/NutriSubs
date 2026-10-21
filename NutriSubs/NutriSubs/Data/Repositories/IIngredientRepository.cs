using NutriSubs.Data.Entities;

namespace NutriSubs.Data.Repositories
{
    // Abstraction over ingredient storage so the substitution lookup service does not
    // depend directly on SQLite. A remote-backed implementation could be substituted later.
    public interface IIngredientRepository
    {
        Task<IngredientEntity?> GetByIdAsync(int id);

        Task<IngredientEntity?> GetByKeyAsync(string key);

        // Resolves a user-entered search term (canonical name or alias, case-insensitive)
        // to its canonical ingredient, or null if nothing matches.
        Task<IngredientEntity?> FindByNameOrAliasAsync(string searchTerm);

        // Parses a free-form recipe ingredient line (e.g. "3 tablespoons calamansi") into
        // quantity/unit/name using IngredientParsingService, then resolves the normalized
        // ingredient name against canonical names and aliases. Does not use substring/fuzzy
        // matching, so "coconut milk" will not incorrectly match "coconut cream".
        Task<IngredientEntity?> FindMatchInTextAsync(string ingredientText);

        Task<List<IngredientAliasEntity>> GetAliasesAsync(int ingredientId);

        Task<List<IngredientEntity>> GetAllActiveAsync();

        // Used by the seeder: inserts the ingredient if Key is new, or updates it if it
        // already exists, without ever creating duplicate rows for the same Key.
        Task<int> UpsertAsync(IngredientEntity ingredient);

        Task UpsertAliasAsync(IngredientAliasEntity alias);
    }
}
