using SQLite;

namespace NutriSubs.Data.Entities
{
    // Tracks the single row representing the current applied schema version.
    // Used by DatabaseInitializer to decide whether to create the schema and/or reseed.
    [Table("SchemaInfo")]
    public class SchemaInfoEntity
    {
        [PrimaryKey]
        public int Id { get; set; } = 1;

        [NotNull]
        public int Version { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
