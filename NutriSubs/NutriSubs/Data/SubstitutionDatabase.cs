using SQLite;

namespace NutriSubs.Data
{
    // Owns the single SQLite connection used by the app. Registered as a singleton via DI
    // so the database file is opened once and reused, instead of being opened per-query
    // or per-repository (which would be both slow and risky for concurrent access).
    public class SubstitutionDatabase
    {
        private readonly SQLiteAsyncConnection _connection;

        public SubstitutionDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "nutrisubs.db3");
            _connection = new SQLiteAsyncConnection(dbPath);
        }

        // Exposed so DatabaseInitializer and repositories can share the same connection.
        // Kept internal to the Data namespace conceptually; repositories should prefer
        // going through this type rather than opening their own connections.
        public SQLiteAsyncConnection Connection => _connection;
    }
}
