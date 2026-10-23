using System.Diagnostics;
using NutriSubs.Data.Seed;
using NutriSubs.Models;

namespace NutriSubs.Services
{
    // Lightweight, dependency-free sanity checks for ingredient text normalization and
    // matching. No test project exists in this solution yet, so these run against the
    // already-existing IngredientParsingService / IngredientNameMatcher logic directly
    // (no database/connection needed) and report results via Debug output. Intended to be
    // invoked manually during development (e.g. from App's constructor while debugging),
    // not part of the shipped UI flow.
    public static class IngredientRecognitionSelfCheck
    {
        private class Case
        {
            public required string Input { get; init; }
            public required string ExpectedNormalizedName { get; init; }
            public bool ExpectMatchAgainstSeedData { get; init; } = true;
        }

        public static void RunAndLog()
        {
            var cases = new List<Case>
            {
                new() { Input = "calamansi", ExpectedNormalizedName = "calamansi" },
                new() { Input = "3 tablespoons calamansi", ExpectedNormalizedName = "calamansi" },
                new() { Input = "3 tbsp calamansi", ExpectedNormalizedName = "calamansi" },
                new() { Input = "CALAMANSI", ExpectedNormalizedName = "calamansi" },
                new() { Input = "  calamansi  ", ExpectedNormalizedName = "calamansi" },
                new() { Input = "1/2 cup all-purpose flour", ExpectedNormalizedName = "all-purpose flour" },
                new() { Input = "kalamansi", ExpectedNormalizedName = "kalamansi" }, // known alias, not the canonical name
                new() { Input = "unobtainium powder", ExpectedNormalizedName = "unobtainium powder", ExpectMatchAgainstSeedData = false },
                // "ham" must not match an unrelated ingredient whose name merely contains those letters
                // (e.g. must not falsely match "shrimp paste" or similar); it is itself unknown here.
                new() { Input = "ham", ExpectedNormalizedName = "ham", ExpectMatchAgainstSeedData = false },
            };

            foreach (var testCase in cases)
            {
                var parsed = IngredientParsingService.Parse(testCase.Input);
                var normalizedOk = parsed.ParseStatus != IngredientParseStatus.Failed
                    && string.Equals(parsed.IngredientName, testCase.ExpectedNormalizedName, StringComparison.OrdinalIgnoreCase);

                var matched = TryFindInSeedData(parsed.IngredientName, out var matchedCanonicalName);
                var matchOk = matched == testCase.ExpectMatchAgainstSeedData;

                Debug.WriteLine(
                    $"[NutriSubs][SelfCheck] input=\"{testCase.Input}\" " +
                    $"normalized=\"{parsed.IngredientName}\" (expected \"{testCase.ExpectedNormalizedName}\", ok={normalizedOk}) " +
                    $"matched={(matched ? matchedCanonicalName : "<none>")} (expectedMatch={testCase.ExpectMatchAgainstSeedData}, ok={matchOk})");
            }
        }

        // Mirrors IngredientRepository's matching order (exact name -> exact alias ->
        // constrained whole-word partial match) but runs purely in-memory against
        // SeedData, so this check has no dependency on SQLite or app startup.
        private static bool TryFindInSeedData(string normalizedName, out string canonicalName)
        {
            canonicalName = string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedName))
                return false;

            foreach (var ingredient in SeedData.Ingredients)
            {
                if (string.Equals(ingredient.Name, normalizedName, StringComparison.OrdinalIgnoreCase))
                {
                    canonicalName = ingredient.Name;
                    return true;
                }

                foreach (var (alias, _) in ingredient.Aliases)
                {
                    if (string.Equals(alias, normalizedName, StringComparison.OrdinalIgnoreCase))
                    {
                        canonicalName = ingredient.Name;
                        return true;
                    }
                }
            }

            var normalizedWords = normalizedName.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var ingredient in SeedData.Ingredients)
            {
                var candidateWords = ingredient.Name.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (IngredientNameMatcher.IsConstrainedPartialMatch(normalizedWords, candidateWords))
                {
                    canonicalName = ingredient.Name;
                    return true;
                }

                foreach (var (alias, _) in ingredient.Aliases)
                {
                    var aliasWords = alias.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (IngredientNameMatcher.IsConstrainedPartialMatch(normalizedWords, aliasWords))
                    {
                        canonicalName = ingredient.Name;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}