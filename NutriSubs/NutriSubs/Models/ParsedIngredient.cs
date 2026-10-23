namespace NutriSubs.Models
{
    // Outcome of parsing a single free-form ingredient line (e.g. from a recipe import
    // or a sample recipe) into structured fields.
    public enum IngredientParseStatus
    {
        // Quantity, unit (if present), and ingredient name were all recognized confidently.
        Success,

        // An ingredient name was recovered, but some parts (e.g. quantity or unit) were
        // ambiguous or not recognized. The original text is preserved either way.
        Partial,

        // No usable ingredient name could be recovered; callers should fall back to
        // the original text and must not fabricate a database match.
        Failed
    }

    // Structured representation of a single recipe ingredient line, e.g.
    // "3 tablespoons calamansi" or "2 medium red onions, chopped".
    public class ParsedIngredient
    {
        // The exact original ingredient line, always preserved for display/troubleshooting.
        public required string OriginalText { get; init; }

        // Lower bound of the recognized quantity (and the only value for non-range quantities).
        public decimal? QuantityMin { get; init; }

        // Upper bound for ranges such as "1-2 cloves garlic". Null when there is no range.
        public decimal? QuantityMax { get; init; }

        public bool IsRange => QuantityMax.HasValue && QuantityMin.HasValue && QuantityMax != QuantityMin;

        // Convenience accessor for the common single-value case.
        public decimal? Quantity => QuantityMin;

        // The exact text that produced the parsed quantity (e.g. "1 1/2", "½", "1-2"),
        // kept so a fraction/range can be displayed as originally written.
        public string? OriginalQuantityText { get; init; }

        // Normalized unit (e.g. "tablespoon"), or null if no unit was recognized.
        public string? Unit { get; init; }

        // The exact unit text as it appeared in the source (e.g. "tbsp", "Tablespoons").
        public string? OriginalUnitText { get; init; }

        // Normalized ingredient name used for database lookup (e.g. "calamansi", "red onion").
        public required string IngredientName { get; init; }

        // Preparation instructions, e.g. "chopped", "minced", "juiced".
        public string? Preparation { get; init; }

        // Descriptive qualifiers that are not part of the canonical ingredient name,
        // e.g. "fresh", "medium", "to taste", "divided".
        public string? AdditionalDescription { get; init; }

        public IngredientParseStatus ParseStatus { get; init; }
    }
}
