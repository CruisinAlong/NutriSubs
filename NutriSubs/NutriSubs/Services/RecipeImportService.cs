using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using NutriSubs.Models;

namespace NutriSubs.Services
{
    // Imports a Recipe from a public recipe web page that exposes Schema.org
    // "Recipe" structured data (JSON-LD). This is intentionally simple:
    // it only understands JSON-LD, it does not render JavaScript, and it
    // does not attempt to bypass logins, paywalls, or bot protection.
    public class RecipeImportService
    {
        private static readonly HttpClient HttpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(15)
            };

            // A normal-looking User-Agent avoids some basic bot blocks that reject
            // requests with no/blank User-Agent. This does not bypass real anti-bot
            // protections such as CAPTCHAs or JavaScript challenges.
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (compatible; NutriSubsApp/1.0; +https://github.com/CruisinAlong/NutriSubs)");

            return client;
        }

        // Validates the URL and attempts to download + parse a recipe from it.
        // Throws RecipeImportException with a user-friendly message on any failure.
        public async Task<Recipe> ImportFromUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new RecipeImportException("Please paste a recipe URL before importing.");

            if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new RecipeImportException("That doesn't look like a valid web address. Please enter a full http:// or https:// URL.");
            }

            string html;
            try
            {
                using var response = await HttpClient.GetAsync(uri);

                if (!response.IsSuccessStatusCode)
                {
                    throw new RecipeImportException(
                        $"The website responded with an error ({(int)response.StatusCode}). The recipe could not be imported.");
                }

                html = await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                throw new RecipeImportException("The request timed out while contacting the website. Please check your connection and try again.");
            }
            catch (HttpRequestException ex)
            {
                throw new RecipeImportException("Could not reach that website. It may be unavailable, or it may be blocking automated requests.", ex);
            }

            var recipe = TryExtractRecipeFromHtml(html, uri);

            if (recipe is null)
            {
                throw new RecipeImportException(
                    "No recipe information could be found on that page. This importer only supports sites that publish structured recipe data (Schema.org Recipe / JSON-LD).");
            }

            return recipe;
        }

        private static Recipe? TryExtractRecipeFromHtml(string html, Uri sourceUri)
        {
            foreach (var jsonBlock in ExtractJsonLdBlocks(html))
            {
                JsonDocument document;
                try
                {
                    document = JsonDocument.Parse(jsonBlock);
                }
                catch (JsonException)
                {
                    // Malformed JSON-LD on the page; skip this block and keep looking.
                    continue;
                }

                using (document)
                {
                    var recipeElement = FindRecipeNode(document.RootElement);
                    if (recipeElement is not null)
                    {
                        return MapJsonToRecipe(recipeElement.Value, sourceUri);
                    }
                }
            }

            return null;
        }

        // Finds all <script type="application/ld+json">...</script> blocks in the page.
        private static IEnumerable<string> ExtractJsonLdBlocks(string html)
        {
            var matches = Regex.Matches(
                html,
                "<script[^>]+type=[\"']application/ld\\+json[\"'][^>]*>(.*?)</script>",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                yield return System.Net.WebUtility.HtmlDecode(match.Groups[1].Value).Trim();
            }
        }

        // Recursively looks for an object whose "@type" is (or includes) "Recipe".
        // Handles plain objects, arrays of objects, and the common "@graph" wrapper.
        private static JsonElement? FindRecipeNode(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    if (element.TryGetProperty("@type", out var typeProp) && IsRecipeType(typeProp))
                    {
                        return element;
                    }

                    if (element.TryGetProperty("@graph", out var graphProp))
                    {
                        var found = FindRecipeNode(graphProp);
                        if (found is not null)
                            return found;
                    }

                    foreach (var property in element.EnumerateObject())
                    {
                        if (property.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
                        {
                            var found = FindRecipeNode(property.Value);
                            if (found is not null)
                                return found;
                        }
                    }

                    return null;

                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        var found = FindRecipeNode(item);
                        if (found is not null)
                            return found;
                    }

                    return null;

                default:
                    return null;
            }
        }

        private static bool IsRecipeType(JsonElement typeProp)
        {
            if (typeProp.ValueKind == JsonValueKind.String)
            {
                return string.Equals(typeProp.GetString(), "Recipe", StringComparison.OrdinalIgnoreCase);
            }

            if (typeProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in typeProp.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String &&
                        string.Equals(item.GetString(), "Recipe", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static Recipe MapJsonToRecipe(JsonElement recipeElement, Uri sourceUri)
        {
            var recipe = new Recipe
            {
                // Negative/time-based Id keeps imported recipes from colliding with sample recipe Ids.
                Id = -Math.Abs(sourceUri.ToString().GetHashCode()),
                Name = GetFirstString(recipeElement, "name") ?? "Imported Recipe",
                Description = GetFirstString(recipeElement, "description") ?? string.Empty,
                PrepTimeMinutes = ParseIsoDurationMinutes(GetFirstString(recipeElement, "prepTime")),
                CookTimeMinutes = ParseIsoDurationMinutes(GetFirstString(recipeElement, "cookTime")),
                Servings = ParseServings(recipeElement),
                Ingredients = ParseIngredients(recipeElement),
                Instructions = ParseInstructions(recipeElement),
                SourceUrl = sourceUri.ToString()
            };

            // If only "totalTime" is present (no separate prep/cook), use it as the cook time
            // so the total shown on the Recipe screen is still meaningful.
            if (recipe.PrepTimeMinutes == 0 && recipe.CookTimeMinutes == 0)
            {
                var totalMinutes = ParseIsoDurationMinutes(GetFirstString(recipeElement, "totalTime"));
                if (totalMinutes > 0)
                    recipe.CookTimeMinutes = totalMinutes;
            }

            return recipe;
        }

        private static string? GetFirstString(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var prop))
                return null;

            return prop.ValueKind switch
            {
                JsonValueKind.String => prop.GetString(),
                JsonValueKind.Array when prop.GetArrayLength() > 0 => prop[0].ValueKind == JsonValueKind.String
                    ? prop[0].GetString()
                    : null,
                JsonValueKind.Number => prop.ToString(),
                _ => null
            };
        }

        // Converts an ISO-8601 duration like "PT1H30M" into total minutes.
        // Returns 0 if the value is missing or cannot be parsed.
        private static int ParseIsoDurationMinutes(string? isoDuration)
        {
            if (string.IsNullOrWhiteSpace(isoDuration))
                return 0;

            var match = Regex.Match(isoDuration.Trim(), @"^P(?:(?<days>\d+)D)?(?:T(?:(?<hours>\d+)H)?(?:(?<minutes>\d+)M)?(?:(?<seconds>\d+)S)?)?$");
            if (!match.Success)
                return 0;

            var days = ParseGroup(match, "days");
            var hours = ParseGroup(match, "hours");
            var minutes = ParseGroup(match, "minutes");

            return (days * 24 * 60) + (hours * 60) + minutes;
        }

        private static int ParseGroup(Match match, string groupName)
        {
            var group = match.Groups[groupName];
            return group.Success && int.TryParse(group.Value, out var value) ? value : 0;
        }

        private static int ParseServings(JsonElement recipeElement)
        {
            if (!recipeElement.TryGetProperty("recipeYield", out var yieldProp))
                return 0;

            // recipeYield can be a number, a numeric string, a descriptive string ("4 servings"),
            // or an array containing any of the above.
            if (yieldProp.ValueKind == JsonValueKind.Array && yieldProp.GetArrayLength() > 0)
            {
                yieldProp = yieldProp[0];
            }

            if (yieldProp.ValueKind == JsonValueKind.Number && yieldProp.TryGetInt32(out var numericYield))
            {
                return numericYield;
            }

            if (yieldProp.ValueKind == JsonValueKind.String)
            {
                var match = Regex.Match(yieldProp.GetString() ?? string.Empty, @"\d+");
                if (match.Success && int.TryParse(match.Value, out var parsedYield))
                {
                    return parsedYield;
                }
            }

            return 0;
        }

        private static List<string> ParseIngredients(JsonElement recipeElement)
        {
            var ingredients = new List<string>();

            if (recipeElement.TryGetProperty("recipeIngredient", out var ingredientsProp) ||
                recipeElement.TryGetProperty("ingredients", out ingredientsProp))
            {
                if (ingredientsProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in ingredientsProp.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            var text = item.GetString();
                            if (!string.IsNullOrWhiteSpace(text))
                                ingredients.Add(text.Trim());
                        }
                    }
                }
                else if (ingredientsProp.ValueKind == JsonValueKind.String)
                {
                    var text = ingredientsProp.GetString();
                    if (!string.IsNullOrWhiteSpace(text))
                        ingredients.Add(text.Trim());
                }
            }

            return ingredients;
        }

        private static List<string> ParseInstructions(JsonElement recipeElement)
        {
            var instructions = new List<string>();

            if (!recipeElement.TryGetProperty("recipeInstructions", out var instructionsProp))
                return instructions;

            CollectInstructionSteps(instructionsProp, instructions);
            return instructions;
        }

        // recipeInstructions is one of the messiest parts of the Schema.org Recipe spec:
        // it may be a single string, an array of strings, an array of HowToStep objects,
        // or an array of HowToSection objects that each contain their own itemListElement.
        private static void CollectInstructionSteps(JsonElement element, List<string> instructions)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    var text = element.GetString();
                    if (!string.IsNullOrWhiteSpace(text))
                        instructions.Add(text.Trim());
                    break;

                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        CollectInstructionSteps(item, instructions);
                    }
                    break;

                case JsonValueKind.Object:
                    if (element.TryGetProperty("itemListElement", out var nested))
                    {
                        CollectInstructionSteps(nested, instructions);
                    }
                    else if (element.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
                    {
                        var stepText = textProp.GetString();
                        if (!string.IsNullOrWhiteSpace(stepText))
                            instructions.Add(stepText.Trim());
                    }
                    else if (element.TryGetProperty("name", out var nameProp) && nameProp.ValueKind == JsonValueKind.String)
                    {
                        var stepName = nameProp.GetString();
                        if (!string.IsNullOrWhiteSpace(stepName))
                            instructions.Add(stepName.Trim());
                    }
                    break;
            }
        }
    }
}
