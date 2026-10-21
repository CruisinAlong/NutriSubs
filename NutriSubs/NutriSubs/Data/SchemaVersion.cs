namespace NutriSubs.Data
{
    // Current schema version for the substitutions database.
    // Bump this and add a branch in DatabaseInitializer.MigrateAsync when the schema changes.
    public static class SchemaVersion
    {
        public const int Current = 1;
    }
}
