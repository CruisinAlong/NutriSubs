using System.Globalization;
using System.Text.RegularExpressions;
using NutriSubs.Models;

namespace NutriSubs.Services
{
    // Single reusable parser for free-form recipe ingredient lines (e.g. "3 tablespoons
    // calamansi"). Used by the substitution lookup service, and reusable by any future
    // ingredient-search or shopping-list feature. Deterministic and UI-independent.
    public static class IngredientParsingService
    {
        // Unit aliases normalized to one canonical internal unit string.
        // Includes a few common container words (can, jar, package, box, bag, stick, head)
        // that behave like units in everyday ingredient lines even though they are not
        // strict measurements.
        private static readonly Dictionary<string, string> UnitAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["tablespoon"] = "tablespoon",
            ["tablespoons"] = "tablespoon",
            ["tbsp"] = "tablespoon",
            ["tbsp."] = "tablespoon",
            ["teaspoon"] = "teaspoon",
            ["teaspoons"] = "teaspoon",
            ["tsp"] = "teaspoon",
            ["tsp."] = "teaspoon",
            ["cup"] = "cup",
            ["cups"] = "cup",
            ["gram"] = "gram",
            ["grams"] = "gram",
            ["g"] = "gram",
            ["kilogram"] = "kilogram",
            ["kilograms"] = "kilogram",
            ["kg"] = "kilogram",
            ["milliliter"] = "milliliter",
            ["milliliters"] = "milliliter",
            ["ml"] = "milliliter",
            ["mL"] = "milliliter",
            ["liter"] = "liter",
            ["liters"] = "liter",
            ["l"] = "liter",
            ["L"] = "liter",
            ["ounce"] = "ounce",
            ["ounces"] = "ounce",
            ["oz"] = "ounce",
            ["pound"] = "pound",
            ["pounds"] = "pound",
            ["lb"] = "pound",
            ["lbs"] = "pound",
            ["piece"] = "piece",
            ["pieces"] = "piece",
            ["pc"] = "piece",
            ["clove"] = "clove",
            ["cloves"] = "clove",
            ["slice"] = "slice",
            ["slices"] = "slice",
            ["pinch"] = "pinch",
            ["pinches"] = "pinch",
            ["bunch"] = "bunch",
            ["bunches"] = "bunch",
            // Common container words that act like units in recipe text.
            ["can"] = "can",
            ["cans"] = "can",
            ["jar"] = "jar",
            ["jars"] = "jar",
            ["package"] = "package",
            ["packages"] = "package",
            ["pack"] = "package",
            ["packs"] = "package",
            ["box"] = "box",
            ["boxes"] = "box",
            ["bag"] = "bag",
            ["bags"] = "bag",
            ["stick"] = "stick",
            ["sticks"] = "stick",
            ["head"] = "head",
            ["heads"] = "head",
        };

        // Size/qualifier words that describe the ingredient but are not part of its
        // canonical name (kept in AdditionalDescription instead of IngredientName).
        private static readonly HashSet<string> DescriptorWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "fresh", "large", "medium", "small", "extra-large", "ripe", "overripe",
            "unsalted", "salted", "divided", "raw", "cooked", "boneless", "skinless",
            "whole", "organic"
        };

        // Preparation methods, kept separate from the ingredient name whenever they
        // appear as a clearly separable qualifier (trailing, after a comma, or as a
        // leading modifier before the ingredient head noun).
        private static readonly HashSet<string> PreparationWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "chopped", "minced", "diced", "sliced", "juiced", "crushed", "grated",
            "peeled", "cubed", "melted", "beaten", "shredded", "mashed", "halved",
            "quartered", "julienned", "zested"
        };

        private static readonly Dictionary<char, decimal> UnicodeFractions = new()
        {
            ['¼'] = 0.25m,
            ['½'] = 0.5m,
            ['¾'] = 0.75m,
            ['?'] = 1m / 3m,
            ['?'] = 2m / 3m,
            ['?'] = 0.125m,
            ['?'] = 0.375m,
            ['?'] = 0.625m,
            ['?'] = 0.875m,
        };

        private static readonly Regex RangeQuantity = new(@"^(?<a>\d+(\.\d+)?)\s*(?:-|to)\s*(?<b>\d+(\.\d+)?)(?=\s|$)", RegexOptions.Compiled);
        private static readonly Regex MixedFraction = new(@"^(?<whole>\d+)\s+(?<num>\d+)/(?<den>\d+)(?=\s|$)", RegexOptions.Compiled);
        private static readonly Regex SimpleFraction = new(@"^(?<num>\d+)/(?<den>\d+)(?=\s|$)", RegexOptions.Compiled);
        private static readonly Regex MixedUnicodeFraction = new(@"^(?<whole>\d+)\s*(?<uni>[¼½¾??????])(?=\s|$)", RegexOptions.Compiled);
        private static readonly Regex UnicodeFractionOnly = new(@"^(?<uni>[¼½¾??????])(?=\s|$)", RegexOptions.Compiled);
        private static readonly Regex DecimalOrInteger = new(@"^(?<num>\d+(\.\d+)?)(?=\s|$)", RegexOptions.Compiled);
        private static readonly Regex IndefiniteSmallAmount = new(@"^(a|an)\s+(?<unit>pinch|dash|splash|handful)\s+of\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ToTasteSuffix = new(@"\s+to\s+taste\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ParentheticalText = new(@"\s*\([^)]*\)", RegexOptions.Compiled);

        // Parses a single free-form ingredient line into structured fields. Never throws;
        // returns ParseStatus.Failed (with the original text preserved) if nothing usable
        // can be recovered.
        public static ParsedIngredient Parse(string? ingredientLine)
        {
            var originalText = ingredientLine ?? string.Empty;

            if (string.IsNullOrWhiteSpace(originalText))
            {
                return new ParsedIngredient
                {
                    OriginalText = originalText,
                    IngredientName = string.Empty,
                    ParseStatus = IngredientParseStatus.Failed
                };
            }

            var working = CollapseWhitespace(originalText.Trim());

            // Remove parenthetical asides (e.g. "(about 1 cup)") - not meaningful for lookup.
            working = ParentheticalText.Replace(working, string.Empty).Trim();

            string? additionalDescription = null;

            var toTasteMatch = ToTasteSuffix.Match(working);
            if (toTasteMatch.Success)
            {
                working = working[..toTasteMatch.Index].Trim();
                additionalDescription = "to taste";
            }

            decimal? quantityMin = null;
            decimal? quantityMax = null;
            string? originalQuantityText = null;
            string? unit = null;
            string? originalUnitText = null;

            var indefiniteMatch = IndefiniteSmallAmount.Match(working);
            if (indefiniteMatch.Success)
            {
                quantityMin = 1;
                quantityMax = 1;
                originalQuantityText = indefiniteMatch.Groups[1].Value;
                unit = NormalizeUnit(indefiniteMatch.Groups["unit"].Value, out originalUnitText);
                working = working[indefiniteMatch.Length..].Trim();
            }
            else if (TryParseLeadingQuantity(working, out quantityMin, out quantityMax, out originalQuantityText, out var remainderAfterQuantity))
            {
                working = remainderAfterQuantity;

                // A recognized unit must immediately follow the quantity; otherwise the
                // number was likely not a measurement quantity (e.g. a product name/size).
                var unitWord = FirstWord(working, out var afterUnitWord);
                if (unitWord is not null && UnitAliases.TryGetValue(StripTrailingPunctuation(unitWord), out var canonicalUnit))
                {
                    unit = canonicalUnit;
                    originalUnitText = unitWord;
                    working = afterUnitWord;
                }
            }

            // Split off a trailing preparation/qualifier clause after a comma,
            // e.g. "2 medium red onions, chopped" -> "chopped" is a preparation note.
            string? preparation = null;
            var commaIndex = working.IndexOf(',');
            if (commaIndex >= 0)
            {
                var trailing = working[(commaIndex + 1)..].Trim();
                working = working[..commaIndex].Trim();

                foreach (var word in SplitWords(trailing))
                {
                    if (PreparationWords.Contains(word))
                        preparation = AppendClause(preparation, word.ToLowerInvariant());
                    else
                        additionalDescription = AppendClause(additionalDescription, word);
                }
            }

            // Strip leading descriptor/preparation words from the remaining text
            // (e.g. "minced garlic" -> preparation "minced", name "garlic";
            // "medium red onions" -> descriptor "medium", name "red onions").
            var words = new List<string>(SplitWords(working));
            while (words.Count > 1)
            {
                var first = words[0];
                if (PreparationWords.Contains(first))
                {
                    preparation = AppendClause(preparation, first.ToLowerInvariant());
                    words.RemoveAt(0);
                }
                else if (DescriptorWords.Contains(first))
                {
                    additionalDescription = AppendClause(additionalDescription, first.ToLowerInvariant());
                    words.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            var ingredientName = Singularize(string.Join(' ', words)).Trim();

            if (string.IsNullOrWhiteSpace(ingredientName))
            {
                return new ParsedIngredient
                {
                    OriginalText = originalText,
                    IngredientName = string.Empty,
                    Preparation = preparation,
                    AdditionalDescription = additionalDescription,
                    ParseStatus = IngredientParseStatus.Failed
                };
            }

            var status = quantityMin is null && unit is null
                ? IngredientParseStatus.Partial
                : IngredientParseStatus.Success;

            // A line with neither quantity nor unit still parses fine if it is exactly an
            // ingredient name with nothing else (e.g. "coconut milk").
            if (status == IngredientParseStatus.Partial && words.Count == SplitWords(originalText).Count())
                status = IngredientParseStatus.Success;

            return new ParsedIngredient
            {
                OriginalText = originalText,
                QuantityMin = quantityMin,
                QuantityMax = quantityMax,
                OriginalQuantityText = originalQuantityText,
                Unit = unit,
                OriginalUnitText = originalUnitText,
                IngredientName = ingredientName,
                Preparation = preparation,
                AdditionalDescription = additionalDescription,
                ParseStatus = status
            };
        }

        private static bool TryParseLeadingQuantity(
            string text,
            out decimal? min,
            out decimal? max,
            out string? originalText,
            out string remainder)
        {
            min = null;
            max = null;
            originalText = null;
            remainder = text;

            var rangeMatch = RangeQuantity.Match(text);
            if (rangeMatch.Success)
            {
                min = decimal.Parse(rangeMatch.Groups["a"].Value, CultureInfo.InvariantCulture);
                max = decimal.Parse(rangeMatch.Groups["b"].Value, CultureInfo.InvariantCulture);
                originalText = rangeMatch.Value;
                remainder = text[rangeMatch.Length..].Trim();
                return true;
            }

            var mixedFractionMatch = MixedFraction.Match(text);
            if (mixedFractionMatch.Success)
            {
                var whole = decimal.Parse(mixedFractionMatch.Groups["whole"].Value, CultureInfo.InvariantCulture);
                var num = decimal.Parse(mixedFractionMatch.Groups["num"].Value, CultureInfo.InvariantCulture);
                var den = decimal.Parse(mixedFractionMatch.Groups["den"].Value, CultureInfo.InvariantCulture);
                min = max = whole + (den == 0 ? 0 : num / den);
                originalText = mixedFractionMatch.Value;
                remainder = text[mixedFractionMatch.Length..].Trim();
                return true;
            }

            var mixedUnicodeMatch = MixedUnicodeFraction.Match(text);
            if (mixedUnicodeMatch.Success)
            {
                var whole = decimal.Parse(mixedUnicodeMatch.Groups["whole"].Value, CultureInfo.InvariantCulture);
                var frac = UnicodeFractions[mixedUnicodeMatch.Groups["uni"].Value[0]];
                min = max = whole + frac;
                originalText = mixedUnicodeMatch.Value;
                remainder = text[mixedUnicodeMatch.Length..].Trim();
                return true;
            }

            var simpleFractionMatch = SimpleFraction.Match(text);
            if (simpleFractionMatch.Success)
            {
                var num = decimal.Parse(simpleFractionMatch.Groups["num"].Value, CultureInfo.InvariantCulture);
                var den = decimal.Parse(simpleFractionMatch.Groups["den"].Value, CultureInfo.InvariantCulture);
                min = max = den == 0 ? 0 : num / den;
                originalText = simpleFractionMatch.Value;
                remainder = text[simpleFractionMatch.Length..].Trim();
                return true;
            }

            var unicodeOnlyMatch = UnicodeFractionOnly.Match(text);
            if (unicodeOnlyMatch.Success)
            {
                min = max = UnicodeFractions[unicodeOnlyMatch.Groups["uni"].Value[0]];
                originalText = unicodeOnlyMatch.Value;
                remainder = text[unicodeOnlyMatch.Length..].Trim();
                return true;
            }

            var decimalMatch = DecimalOrInteger.Match(text);
            if (decimalMatch.Success)
            {
                min = max = decimal.Parse(decimalMatch.Groups["num"].Value, CultureInfo.InvariantCulture);
                originalText = decimalMatch.Value;
                remainder = text[decimalMatch.Length..].Trim();
                return true;
            }

            return false;
        }

        private static string? NormalizeUnit(string rawUnit, out string? originalUnitText)
        {
            originalUnitText = rawUnit;
            return UnitAliases.TryGetValue(StripTrailingPunctuation(rawUnit), out var canonical) ? canonical : rawUnit;
        }

        private static string? FirstWord(string text, out string remainder)
        {
            text = text.Trim();
            if (text.Length == 0)
            {
                remainder = text;
                return null;
            }

            var spaceIndex = text.IndexOf(' ');
            if (spaceIndex < 0)
            {
                remainder = string.Empty;
                return text;
            }

            remainder = text[(spaceIndex + 1)..].Trim();
            return text[..spaceIndex];
        }

        private static IEnumerable<string> SplitWords(string text) =>
            text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(StripTrailingPunctuation)
                .Where(w => w.Length > 0);

        private static string StripTrailingPunctuation(string word) => word.Trim('.', ',', ';', ':');

        private static string CollapseWhitespace(string text) => Regex.Replace(text, @"\s+", " ");

        private static string? AppendClause(string? existing, string addition) =>
            string.IsNullOrEmpty(existing) ? addition : $"{existing}, {addition}";

        // Simple heuristic singularization limited to the final word of the ingredient
        // name (e.g. "onions" -> "onion", "eggs" -> "egg"). Deliberately conservative to
        // avoid mangling words that already end in "s" as part of their singular form.
        private static string Singularize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
                return text;

            var last = words[^1];

            if (last.Length > 3
                && last.EndsWith('s')
                && !last.EndsWith("ss", StringComparison.OrdinalIgnoreCase)
                && !last.EndsWith("us", StringComparison.OrdinalIgnoreCase)
                && !last.EndsWith("is", StringComparison.OrdinalIgnoreCase))
            {
                words[^1] = last[..^1];
            }

            return string.Join(' ', words);
        }
    }
}
