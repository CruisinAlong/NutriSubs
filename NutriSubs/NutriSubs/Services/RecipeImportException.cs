namespace NutriSubs.Services
{
    // Thrown when a recipe cannot be imported from a URL for any user-facing reason
    // (bad URL, network failure, unsupported site, missing/invalid structured data).
    // The Message is written to be shown directly to the user.
    public class RecipeImportException : Exception
    {
        public RecipeImportException(string message) : base(message)
        {
        }

        public RecipeImportException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
