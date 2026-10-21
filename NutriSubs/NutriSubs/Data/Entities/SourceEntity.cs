using SQLite;

namespace NutriSubs.Data.Entities
{
    // Provenance for a substitution recommendation. Referenced by SubstitutionEntity.SourceId.
    [Table("Sources")]
    public class SourceEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Stable key used to make reseeding idempotent (e.g. "healthline-butter-substitutes").
        [Indexed(Unique = true)]
        public string? SeedKey { get; set; }

        [NotNull]
        public string SourceName { get; set; } = string.Empty;

        public string? SourceUrl { get; set; }

        public string? ReferenceTitle { get; set; }

        public DateTime? LastVerifiedAt { get; set; }

        public string? Notes { get; set; }
    }
}
