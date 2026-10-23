using System.Diagnostics;
using System.Diagnostics;
using NutriSubs.Data;
using NutriSubs.Data.Entities;
using NutriSubs.Data.Repositories;
using NutriSubs.Models;

namespace NutriSubs.Services
{
    // Reusable, UI-independent lookup service so recipe screens, ingredient search, or a
    // future shopping-list feature can all share the same substitution logic.
    public interface ISubstitutionLookupService
    {
        // Resolves a search term (canonical name or alias, e.g. "talong") and returns
        // its verified, active substitution options, optionally filtered by use case.
        Task<SubstitutionLookupResult> FindSubstitutesAsync(string searchTerm, SubstitutionUseCase? useCase = null);
    }

    public class SubstitutionLookupService : ISubstitutionLookupService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ISubstitutionRepository _substitutionRepository;
        private readonly DatabaseInitializer _databaseInitializer;

        public SubstitutionLookupService(
            IIngredientRepository ingredientRepository,
            ISubstitutionRepository substitutionRepository,
            DatabaseInitializer databaseInitializer)
        {
            _ingredientRepository = ingredientRepository;
            _substitutionRepository = substitutionRepository;
            _databaseInitializer = databaseInitializer;
        }

        public async Task<SubstitutionLookupResult> FindSubstitutesAsync(string searchTerm, SubstitutionUseCase? useCase = null)
        {
            await _databaseInitializer.InitializeAsync();

            // Try an exact name/alias match first (covers plain search terms like "coconut milk"),
            // then fall back to full quantity/unit-aware parsing for free-form recipe lines
            // like "3 tablespoons calamansi".
            var ingredient = await _ingredientRepository.FindByNameOrAliasAsync(searchTerm)
                ?? await _ingredientRepository.FindMatchInTextAsync(searchTerm);

            if (ingredient is null)
            {
                LogLookupFailure(searchTerm);
                return SubstitutionLookupResult.NotFound();
            }

            Debug.WriteLine(
                $"[NutriSubs] Ingredient lookup: original=\"{searchTerm}\" -> matched=\"{ingredient.Name}\" (Id={ingredient.Id}).");

            var substitutions = await _substitutionRepository.GetForIngredientAsync(ingredient.Id);

            // Policy: only verified, active recommendations are shown in normal results.
            var verified = substitutions.Where(s => s.Substitution.VerificationStatus == SubstitutionVerificationStatus.Verified);

            if (useCase is not null)
                verified = verified.Where(s => s.UseCases.Contains(useCase.Value) || s.UseCases.Contains(SubstitutionUseCase.General));

            var ranked = await RankAsync(verified.ToList());

            if (ranked.Count == 0)
            {
                // Recognized ingredient, but no substitutes available for it (distinct from
                // "not recognized" - the UI should show a different message for this case).
                Debug.WriteLine($"[NutriSubs] Ingredient \"{ingredient.Name}\" recognized but has no verified substitutions.");
                return SubstitutionLookupResult.Empty(ingredient.Name);
            }

            return new SubstitutionLookupResult
            {
                IngredientFound = true,
                ResolvedIngredientName = ingredient.Name,
                Options = ranked
            };
        }

        // Diagnostic logging only (Debug output), explaining why matching failed so the
        // cause (bad parse vs. truly unknown ingredient) is easy to see during development.
        private static void LogLookupFailure(string searchTerm)
        {
            var parsed = IngredientParsingService.Parse(searchTerm);

            var reason = parsed.ParseStatus == IngredientParseStatus.Failed
                ? "could not parse any ingredient name out of the text"
                : $"parsed ingredient name \"{parsed.IngredientName}\" did not match any known ingredient or alias";

            Debug.WriteLine(
                $"[NutriSubs] Ingredient lookup failed: original=\"{searchTerm}\" normalized=\"{parsed.IngredientName}\" reason={reason}.");
        }

        private async Task<List<SubstitutionOption>> RankAsync(List<SubstitutionWithDetails> substitutions)
        {
            var options = new List<(SubstitutionOption Option, int Score)>();

            foreach (var substitution in substitutions)
            {
                var components = new List<SubstitutionComponentOption>();

                foreach (var component in substitution.Components)
                {
                    var substituteIngredient = await _ingredientRepository.GetByIdAsync(component.SubstituteIngredientId);
                    if (substituteIngredient is null)
                        continue;

                    components.Add(new SubstitutionComponentOption
                    {
                        IngredientName = substituteIngredient.Name,
                        RatioNumerator = component.RatioNumerator,
                        RatioDenominator = component.RatioDenominator,
                        OriginalUnit = component.OriginalUnit,
                        SubstituteUnit = component.SubstituteUnit
                    });
                }

                if (components.Count == 0)
                    continue;

                // Deterministic ranking: verified compatible-use-case substitutions with a
                // single direct ingredient and clear ratio/instructions rank first;
                // compound (multi-component) substitutions rank after direct ones.
                var score = 0;
                score += components.Count == 1 ? 100 : 50;
                score += !string.IsNullOrWhiteSpace(substitution.Substitution.Instructions) ? 10 : 0;
                score += substitution.Source is not null ? 5 : 0;

                options.Add((new SubstitutionOption
                {
                    Instructions = substitution.Substitution.Instructions ?? string.Empty,
                    FlavorImpact = substitution.Substitution.FlavorImpact,
                    TextureImpact = substitution.Substitution.TextureImpact,
                    Limitations = substitution.Substitution.Limitations,
                    Components = components,
                    UseCases = substitution.UseCases,
                    VerificationStatus = substitution.Substitution.VerificationStatus,
                    SourceName = substitution.Source?.SourceName,
                    SourceUrl = substitution.Source?.SourceUrl
                }, score));
            }

            return options
                .OrderByDescending(o => o.Score)
                .Select(o => o.Option)
                .ToList();
        }
    }
}