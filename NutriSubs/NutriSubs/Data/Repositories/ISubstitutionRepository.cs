using NutriSubs.Data.Entities;

namespace NutriSubs.Data.Repositories
{
    // A fully-loaded substitution: the base record plus its replacement component(s)
    // and applicable use cases, ready for ranking/filtering by the lookup service.
    public class SubstitutionWithDetails
    {
        public required SubstitutionEntity Substitution { get; init; }

        public required List<SubstitutionComponentEntity> Components { get; init; }

        public required List<SubstitutionUseCase> UseCases { get; init; }

        public SourceEntity? Source { get; init; }
    }

    public interface ISubstitutionRepository
    {
        // Returns all active substitutions for the given original ingredient, including
        // their components, use cases, and source, suitable for ranking/display.
        Task<List<SubstitutionWithDetails>> GetForIngredientAsync(int ingredientId);

        Task<int> UpsertSubstitutionAsync(SubstitutionEntity substitution);

        Task ReplaceComponentsAsync(int substitutionId, IEnumerable<SubstitutionComponentEntity> components);

        Task ReplaceUseCasesAsync(int substitutionId, IEnumerable<SubstitutionUseCase> useCases);

        Task<int> UpsertSourceAsync(SourceEntity source);
    }
}
