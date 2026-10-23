namespace NutriSubs.Data
{
    // Thrown when the substitution database fails to initialize. Callers (e.g. the
    // lookup service) can catch this to show a friendly "unavailable" state instead
    // of crashing the app.
    public class SubstitutionDatabaseException : Exception
    {
        public SubstitutionDatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
