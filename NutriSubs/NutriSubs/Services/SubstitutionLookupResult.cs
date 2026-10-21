using NutriSubs.Data.Entities;

namespace NutriSubs.Services
{
    // A single substitution option ready for display: the original ingredient name,
    // the replacement component(s) (supporting compound substitutions like milk+vinegar),
    // and the supporting guidance/limitations/source needed by the UI.
    public class SubstitutionOption
    {
        public required string Instructions { get; init; }
        public string? FlavorImpact { get; init; }
        public string? TextureImpact { get; init; }
        public string? Limitations { get; init; }
        public required List<SubstitutionComponentOption> Components { get; init; }
        public required List<SubstitutionUseCase> UseCases { get; init; }
        public required SubstitutionVerificationStatus VerificationStatus { get; init; }
        public string? SourceName { get; init; }
        public string? SourceUrl { get; init; }
    }

    public class SubstitutionComponentOption
    {
        public required string IngredientName { get; init; }
        public int RatioNumerator { get; init; }
        public int RatioDenominator { get; init; }
        public string? OriginalUnit { get; init; }
        public string? SubstituteUnit { get; init; }
    }

    // Outcome of a lookup request. Kept explicit (rather than just an empty list) so the
    // UI can distinguish "ingredient not found" from "no verified substitutions available".
    public class SubstitutionLookupResult
    {
        public bool IngredientFound { get; init; }
        public string? ResolvedIngredientName { get; init; }
        public List<SubstitutionOption> Options { get; init; } = new();

        public static SubstitutionLookupResult NotFound() => new() { IngredientFound = false };

        public static SubstitutionLookupResult Empty(string resolvedIngredientName) => new()
        {
            IngredientFound = true,
            ResolvedIngredientName = resolvedIngredientName,
            Options = new List<SubstitutionOption>()
        };
    }
}
