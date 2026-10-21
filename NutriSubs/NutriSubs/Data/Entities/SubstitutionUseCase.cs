namespace NutriSubs.Data.Entities
{
    // Cooking/usage context a substitution is suitable for.
    // Kept as a fixed enum for the MVP since the use case list is small and well understood;
    // revisit as a lookup table only if the list needs to grow beyond simple, known categories.
    public enum SubstitutionUseCase
    {
        General = 0,
        Baking = 1,
        Frying = 2,
        Sauces = 3,
        Soups = 4,
        Drinks = 5
    }
}
