using SQLite;

namespace NutriSubs.Data.Entities
{
    // One replacement ingredient within a substitution. A substitution with a single
    // component is a simple 1-for-1 swap (e.g. butter -> margarine). A substitution
    // with multiple components represents a combination replacement
    // (e.g. milk -> 1 cup milk substitute made from 1 tbsp vinegar + milk to fill).
    [Table("SubstitutionComponents")]
    public class SubstitutionComponentEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, Indexed]
        public int SubstitutionId { get; set; }

        [NotNull, Indexed]
        public int SubstituteIngredientId { get; set; }

        // Explicit ratio as a fraction to avoid floating point ambiguity
        // (e.g. 3/4 cup substitute per 1 cup original => RatioNumerator=3, RatioDenominator=4).
        [NotNull]
        public int RatioNumerator { get; set; } = 1;

        [NotNull]
        public int RatioDenominator { get; set; } = 1;

        public string? OriginalUnit { get; set; }

        public string? SubstituteUnit { get; set; }

        // Order in which components are listed/combined for compound substitutions.
        public int SortOrder { get; set; }
    }
}
