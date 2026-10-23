using NutriSubs.Data.Entities;
using NutriSubs.Data.Repositories;
using NutriSubs.Data.Seed;
using SQLite;

namespace NutriSubs.Data
{
    // Owns schema creation, versioning, and (idempotent) seeding. Called once during
    // app startup via dependency injection, not from Views or on every navigation.
    public class DatabaseInitializer
    {
        private readonly SubstitutionDatabase _database;
        private readonly DatabaseSeeder _seeder;

        private Task? _initializationTask;
        private readonly object _lock = new();

        public DatabaseInitializer(SubstitutionDatabase database, DatabaseSeeder seeder)
        {
            _database = database;
            _seeder = seeder;
        }

        // Safe to call multiple times (e.g. from multiple pages); the underlying work
        // only runs once per app session.
        public Task InitializeAsync()
        {
            lock (_lock)
            {
                _initializationTask ??= InitializeInternalAsync();
                return _initializationTask;
            }
        }

        private async Task InitializeInternalAsync()
        {
            try
            {
                await CreateSchemaAsync();
                await MigrateAsync();
                await _seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                // Database issues shouldn't crash the app; the substitution feature degrades
                // gracefully (lookup returns empty results) and the rest of the app keeps working.
                System.Diagnostics.Debug.WriteLine($"[NutriSubs] Substitution database initialization failed: {ex}");
                throw new SubstitutionDatabaseException(
                    "The substitution database could not be initialized.", ex);
            }
        }

        private async Task CreateSchemaAsync()
        {
            var connection = _database.Connection;

            await connection.CreateTableAsync<SchemaInfoEntity>();
            await connection.CreateTableAsync<IngredientEntity>();
            await connection.CreateTableAsync<IngredientAliasEntity>();
            await connection.CreateTableAsync<SourceEntity>();
            await connection.CreateTableAsync<SubstitutionEntity>();
            await connection.CreateTableAsync<SubstitutionComponentEntity>();
            await connection.CreateTableAsync<SubstitutionUseCaseEntity>();
        }

        private async Task MigrateAsync()
        {
            var connection = _database.Connection;

            var info = await connection.Table<SchemaInfoEntity>().FirstOrDefaultAsync();

            if (info is null)
            {
                // Fresh database: record the current version directly, no migration needed.
                await connection.InsertAsync(new SchemaInfoEntity { Id = 1, Version = SchemaVersion.Current });
                return;
            }

            if (info.Version == SchemaVersion.Current)
                return;

            // Migration steps would go here, e.g.:
            // if (info.Version < 2) { await connection.ExecuteAsync("ALTER TABLE ..."); info.Version = 2; }
            // Each step should be additive and run inside a transaction where it modifies data,
            // preserving existing user data rather than dropping/recreating tables.

            info.Version = SchemaVersion.Current;
            info.UpdatedAt = DateTime.UtcNow;
            await connection.UpdateAsync(info);
        }
    }
}
