namespace NutriSubs.Services
{
    // Small, deterministic helper used for the constrained partial-match step of ingredient
    // lookup. Kept separate (and dependency-free) so it can be unit tested in isolation from
    // SQLite, and so the "no loose substring matches" rule is defined in exactly one place.
    public static class IngredientNameMatcher
    {
        // True only when one word sequence is a whole-word prefix of the other
        // (e.g. "calamansi" vs "calamansi juice"). This deliberately rejects cases like
        // "coconut milk" vs "coconut cream", which only share a prefix word, not a full
        // whole-word prefix relationship.
        public static bool IsConstrainedPartialMatch(string[] normalizedWords, string[] candidateWords)
        {
            return StartsWithWords(candidateWords, normalizedWords) ||
                   StartsWithWords(normalizedWords, candidateWords);
        }

        private static bool StartsWithWords(string[] longer, string[] shorter)
        {
            if (shorter.Length == 0 || shorter.Length > longer.Length)
                return false;

            for (var i = 0; i < shorter.Length; i++)
            {
                if (!string.Equals(longer[i], shorter[i], StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }
    }
}
