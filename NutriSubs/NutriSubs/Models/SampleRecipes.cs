using System.Collections.Generic;

namespace NutriSubs.Models
{
    // Local, in-memory sample recipe data used to demonstrate navigation
    // between the Home screen and the Recipe screen.
    // No external database or API is used here.
    public static class SampleRecipes
    {
        public static List<Recipe> All { get; } = new()
        {
            new Recipe
            {
                Id = 1,
                Name = "Garlic Fried Rice (Sinangag)",
                Description = "A quick and affordable Filipino breakfast staple made with day-old rice and plenty of garlic.",
                PrepTimeMinutes = 5,
                CookTimeMinutes = 10,
                Servings = 2,
                Ingredients = new List<string>
                {
                    "2 cups day-old cooked rice",
                    "4 cloves garlic, minced",
                    "2 tbsp cooking oil",
                    "Salt, to taste",
                    "Spring onions for garnish (optional)"
                },
                Instructions = new List<string>
                {
                    "Heat oil in a pan over medium heat.",
                    "Add minced garlic and saute until golden and fragrant.",
                    "Add the rice, breaking up any clumps.",
                    "Stir-fry until the rice is heated through and evenly coated with garlic oil.",
                    "Season with salt and garnish with spring onions before serving."
                }
            },
            new Recipe
            {
                Id = 2,
                Name = "Tortang Talong (Eggplant Omelette)",
                Description = "A budget-friendly, protein-packed dish of grilled eggplant dipped in egg and pan-fried.",
                PrepTimeMinutes = 10,
                CookTimeMinutes = 10,
                Servings = 2,
                Ingredients = new List<string>
                {
                    "2 long eggplants",
                    "2 eggs, beaten",
                    "Salt and pepper, to taste",
                    "2 tbsp cooking oil"
                },
                Instructions = new List<string>
                {
                    "Grill or roast the eggplants until the skin is charred and the flesh is soft.",
                    "Let cool, then peel off the skin and flatten the flesh with a fork, keeping the stem intact.",
                    "Season the beaten eggs with salt and pepper.",
                    "Dip each flattened eggplant into the egg mixture.",
                    "Pan-fry in oil over medium heat until golden on both sides."
                }
            },
            new Recipe
            {
                Id = 3,
                Name = "Monggo Guisado (Mung Bean Stew)",
                Description = "A hearty, inexpensive one-pot stew made from mung beans, perfect for students on a budget.",
                PrepTimeMinutes = 10,
                CookTimeMinutes = 30,
                Servings = 4,
                Ingredients = new List<string>
                {
                    "1 cup mung beans (monggo), rinsed",
                    "1 onion, chopped",
                    "2 cloves garlic, minced",
                    "1 tomato, chopped",
                    "2 cups water or broth",
                    "Salt and pepper, to taste"
                },
                Instructions = new List<string>
                {
                    "Boil the mung beans in water until soft, about 20 minutes.",
                    "In a separate pan, saute garlic, onion, and tomato until fragrant.",
                    "Add the cooked mung beans along with their liquid.",
                    "Simmer for 10 more minutes, mashing some beans to thicken the stew.",
                    "Season with salt and pepper before serving."
                }
            }
        };

        public static Recipe? GetById(int id)
        {
            return All.Find(r => r.Id == id);
        }
    }
}
