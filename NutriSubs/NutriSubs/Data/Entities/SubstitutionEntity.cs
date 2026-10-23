using SQLite;

namespace NutriSubs.Data.Entities
{
    // Represents one substitution recommendation for an original ingredient.
    // The actual replacement ingredient(s) and their ratios live in SubstitutionComponentEntity,
    // which supports both simple (single ingredient) and compound (e.g. milk + vinegar) substitutions
    // without duplicating that information here.
    [Table("Substitutions")]
    public class SubstitutionEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Stable, human-readable key used only to make seeding idempotent
        // (e.g. "butter-to-margarine-baking"). Null for substitutions added outside seeding.
        [Indexed(Unique = true)]
        public string? SeedKey { get; set; }

        [NotNull, Indexed]
        public int OriginalIngredientId { get; set; }

        public string? Instructions { get; set; }

        public string? FlavorImpact { get; set; }

        public string? TextureImpact { get; set; }

        public string? Limitations { get; set; }

        [NotNull]
        public SubstitutionVerificationStatus VerificationStatus { get; set; } = SubstitutionVerificationStatus.Unverified;

        public int? SourceId { get; set; }

        [NotNull]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
