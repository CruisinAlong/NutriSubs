using System.Collections.Generic;

namespace NutriSubs.Models
{
    // Simple, reusable recipe model.
    // Shared by the Home screen (for listing/browsing) and the Recipe screen (for details).
    public class Recipe
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int PrepTimeMinutes { get; set; }

        public int CookTimeMinutes { get; set; }

        public int Servings { get; set; }

        public List<string> Ingredients { get; set; } = new();

        public List<string> Instructions { get; set; } = new();

        // Convenience text used on the Home screen recipe cards.
        public string PrepTimeDisplay => $"{PrepTimeMinutes + CookTimeMinutes} min total";
    }
}
