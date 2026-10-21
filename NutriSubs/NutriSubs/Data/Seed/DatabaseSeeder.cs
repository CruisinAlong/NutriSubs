using NutriSubs.Data.Entities;
using NutriSubs.Data.Repositories;

namespace NutriSubs.Data.Seed
{
    // Declarative seed record used only during database seeding. Kept separate from
    // the SQLite entities so the curated dataset reads clearly and stays in one place
    // instead of being scattered across ViewModels or UI code.
    public class SeedIngredient
    {
        public required string Key { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public string? Category { get; init; }
        public string? DefaultUnit { get; init; }
        public List<(string Alias, string LanguageCode)> Aliases { get; init; } = new();
    }

    public class SeedSubstitutionComponent
    {
        public required string SubstituteIngredientKey { get; init; }
        public int RatioNumerator { get; init; } = 1;
        public int RatioDenominator { get; init; } = 1;
        public string? OriginalUnit { get; init; }
        public string? SubstituteUnit { get; init; }
    }

    public class SeedSubstitution
    {
        public required string SeedKey { get; init; }
        public required string OriginalIngredientKey { get; init; }
        public required List<SeedSubstitutionComponent> Components { get; init; }
        public required List<SubstitutionUseCase> UseCases { get; init; }
        public string? Instructions { get; init; }
        public string? FlavorImpact { get; init; }
        public string? TextureImpact { get; init; }
        public string? Limitations { get; init; }
        public SubstitutionVerificationStatus VerificationStatus { get; init; } = SubstitutionVerificationStatus.Verified;
        public string? SourceKey { get; init; }
    }

    public class SeedSource
    {
        public required string SeedKey { get; init; }
        public required string SourceName { get; init; }
        public string? SourceUrl { get; init; }
        public string? ReferenceTitle { get; init; }
        public string? Notes { get; init; }
    }

    // Idempotent seeder: running this multiple times (e.g. on every app start) upserts
    // by stable Key/SeedKey instead of inserting duplicate rows.
    public class DatabaseSeeder
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ISubstitutionRepository _substitutionRepository;

        public DatabaseSeeder(IIngredientRepository ingredientRepository, ISubstitutionRepository substitutionRepository)
        {
            _ingredientRepository = ingredientRepository;
            _substitutionRepository = substitutionRepository;
        }

        public async Task SeedAsync()
        {
            var ingredientIds = new Dictionary<string, int>();

            foreach (var seedIngredient in SeedData.Ingredients)
            {
                var entity = new IngredientEntity
                {
                    Key = seedIngredient.Key,
                    Name = seedIngredient.Name,
                    Description = seedIngredient.Description,
                    Category = seedIngredient.Category,
                    DefaultUnit = seedIngredient.DefaultUnit,
                    IsActive = true
                };

                var id = await _ingredientRepository.UpsertAsync(entity);
                ingredientIds[seedIngredient.Key] = id;

                foreach (var (alias, languageCode) in seedIngredient.Aliases)
                {
                    await _ingredientRepository.UpsertAliasAsync(new IngredientAliasEntity
                    {
                        IngredientId = id,
                        Alias = alias,
                        LanguageCode = languageCode
                    });
                }
            }

            var sourceIds = new Dictionary<string, int>();

            foreach (var seedSource in SeedData.Sources)
            {
                var id = await _substitutionRepository.UpsertSourceAsync(new SourceEntity
                {
                    SeedKey = seedSource.SeedKey,
                    SourceName = seedSource.SourceName,
                    SourceUrl = seedSource.SourceUrl,
                    ReferenceTitle = seedSource.ReferenceTitle,
                    Notes = seedSource.Notes,
                    LastVerifiedAt = DateTime.UtcNow
                });

                sourceIds[seedSource.SeedKey] = id;
            }

            foreach (var seedSubstitution in SeedData.Substitutions)
            {
                if (!ingredientIds.TryGetValue(seedSubstitution.OriginalIngredientKey, out var originalId))
                {
                    // Defensive: skip rather than crash startup if a seed entry references
                    // an ingredient key that doesn't exist (e.g. a typo during data entry).
                    continue;
                }

                int? sourceId = null;
                if (seedSubstitution.SourceKey is not null && sourceIds.TryGetValue(seedSubstitution.SourceKey, out var resolvedSourceId))
                {
                    sourceId = resolvedSourceId;
                }

                var substitutionId = await _substitutionRepository.UpsertSubstitutionAsync(new SubstitutionEntity
                {
                    SeedKey = seedSubstitution.SeedKey,
                    OriginalIngredientId = originalId,
                    Instructions = seedSubstitution.Instructions,
                    FlavorImpact = seedSubstitution.FlavorImpact,
                    TextureImpact = seedSubstitution.TextureImpact,
                    Limitations = seedSubstitution.Limitations,
                    VerificationStatus = seedSubstitution.VerificationStatus,
                    SourceId = sourceId,
                    IsActive = true
                });

                var components = new List<SubstitutionComponentEntity>();
                var sortOrder = 0;

                foreach (var component in seedSubstitution.Components)
                {
                    if (!ingredientIds.TryGetValue(component.SubstituteIngredientKey, out var substituteId))
                        continue;

                    components.Add(new SubstitutionComponentEntity
                    {
                        SubstituteIngredientId = substituteId,
                        RatioNumerator = component.RatioNumerator,
                        RatioDenominator = component.RatioDenominator,
                        OriginalUnit = component.OriginalUnit,
                        SubstituteUnit = component.SubstituteUnit,
                        SortOrder = sortOrder++
                    });
                }

                await _substitutionRepository.ReplaceComponentsAsync(substitutionId, components);
                await _substitutionRepository.ReplaceUseCasesAsync(substitutionId, seedSubstitution.UseCases);
            }
        }
    }
}
