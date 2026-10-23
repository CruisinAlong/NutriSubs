using SQLite;

namespace NutriSubs.Data.Entities
{
    // Alternative name for an ingredient (English, Filipino, or common spelling variants).
    // Multiple aliases can point to the same IngredientId.
    [Table("IngredientAliases")]
    public class IngredientAliasEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, Indexed]
        public int IngredientId { get; set; }

        [NotNull, Indexed]
        public string Alias { get; set; } = string.Empty;

        // "en", "fil", etc. Kept as a simple string for MVP simplicity.
        public string LanguageCode { get; set; } = "en";

        // e.g. "common", "regional", "misspelling" - optional categorization of the alias.
        public string? AliasType { get; set; }
    }
}
