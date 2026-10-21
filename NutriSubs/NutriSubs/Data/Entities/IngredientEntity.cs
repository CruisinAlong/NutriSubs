using SQLite;

namespace NutriSubs.Data.Entities
{
    // Canonical ingredient record. Aliases (including Filipino names) are stored
    // separately in IngredientAliasEntity so multiple names can resolve to one ingredient.
    [Table("Ingredients")]
    public class IngredientEntity
    {
        [PrimaryKey]
        public int Id { get; set; }

        // Stable, code-level key used by the seeder to find/update ingredients idempotently
        // without depending on the (user-facing, possibly-edited) Name field.
        [NotNull, Indexed(Unique = true)]
        public string Key { get; set; } = string.Empty;

        [NotNull, Indexed(Unique = true)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Free-text category kept simple for the MVP (e.g. "Dairy", "Baking", "Protein").
        public string? Category { get; set; }

        public string? DefaultUnit { get; set; }

        [NotNull]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
