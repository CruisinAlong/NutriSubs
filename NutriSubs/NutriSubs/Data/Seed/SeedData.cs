using NutriSubs.Data.Entities;

namespace NutriSubs.Data.Seed
{
    // Curated MVP seed dataset: ~35 ingredients and ~55 substitutions.
    //
    // Sourcing note: these are widely documented, standard culinary substitution
    // ratios/techniques (e.g. the classic "milk + acid = buttermilk substitute" trick,
    // or 1:1 butter/margarine swaps) repeated consistently across many mainstream cooking
    // references, not copied from one specific article. Rather than inventing a precise
    // article title/URL we cannot verify, each is attributed to a general, honestly
    // described source. Entries we are less confident generalize well across recipes
    // are marked Unverified so the lookup service can exclude them from default results.
    public static class SeedData
    {
        public static readonly List<SeedSource> Sources = new()
        {
            new SeedSource
            {
                SeedKey = "general-culinary-knowledge",
                SourceName = "General culinary reference knowledge",
                SourceUrl = null,
                ReferenceTitle = "Standard baking/cooking substitution ratios widely documented across mainstream cooking references",
                Notes = "Used for well-established substitutions (e.g. buttermilk-from-milk-and-acid, 1:1 butter/margarine) that are consistent across multiple independent culinary sources rather than one specific article."
            },
            new SeedSource
            {
                SeedKey = "filipino-home-cooking-practice",
                SourceName = "Common Filipino home-cooking practice",
                SourceUrl = null,
                ReferenceTitle = "Everyday ingredient substitutions used in Filipino households",
                Notes = "Reflects common, widely-practiced substitutions in Filipino home cooking (e.g. calamansi for vinegar/lemon). Included for local relevance; treat as a starting point rather than a professionally tested reference."
            }
        };

        public static readonly List<SeedIngredient> Ingredients = new()
        {
            // Dairy
            new SeedIngredient { Key = "milk", Name = "Milk", Category = "Dairy", DefaultUnit = "cup" },
            new SeedIngredient { Key = "buttermilk", Name = "Buttermilk", Category = "Dairy", DefaultUnit = "cup" },
            new SeedIngredient { Key = "butter", Name = "Butter", Category = "Dairy", DefaultUnit = "cup" },
            new SeedIngredient { Key = "margarine", Name = "Margarine", Category = "Dairy", DefaultUnit = "cup" },
            new SeedIngredient { Key = "heavy-cream", Name = "Heavy Cream", Category = "Dairy", DefaultUnit = "cup",
                Aliases = new() { ("heavy whipping cream", "en") } },
            new SeedIngredient { Key = "plain-yogurt", Name = "Plain Yogurt", Category = "Dairy", DefaultUnit = "cup" },
            new SeedIngredient { Key = "sour-cream", Name = "Sour Cream", Category = "Dairy", DefaultUnit = "cup" },
            new SeedIngredient { Key = "evaporated-milk", Name = "Evaporated Milk", Category = "Dairy", DefaultUnit = "cup" },
            new SeedIngredient { Key = "condensed-milk", Name = "Condensed Milk", Category = "Dairy", DefaultUnit = "cup" },

            // Baking
            new SeedIngredient { Key = "egg", Name = "Egg", Category = "Baking", DefaultUnit = "piece",
                Aliases = new() { ("itlog", "fil") } },
            new SeedIngredient { Key = "all-purpose-flour", Name = "All-Purpose Flour", Category = "Baking", DefaultUnit = "cup",
                Aliases = new() { ("harina", "fil") } },
            new SeedIngredient { Key = "baking-powder", Name = "Baking Powder", Category = "Baking", DefaultUnit = "tsp" },
            new SeedIngredient { Key = "baking-soda", Name = "Baking Soda", Category = "Baking", DefaultUnit = "tsp" },
            new SeedIngredient { Key = "cornstarch", Name = "Cornstarch", Category = "Baking", DefaultUnit = "tbsp" },
            new SeedIngredient { Key = "vanilla-extract", Name = "Vanilla Extract", Category = "Baking", DefaultUnit = "tsp" },

            // Sweeteners
            new SeedIngredient { Key = "white-sugar", Name = "White Sugar", Category = "Sweetener", DefaultUnit = "cup",
                Aliases = new() { ("asukal", "fil") } },
            new SeedIngredient { Key = "brown-sugar", Name = "Brown Sugar", Category = "Sweetener", DefaultUnit = "cup" },
            new SeedIngredient { Key = "honey", Name = "Honey", Category = "Sweetener", DefaultUnit = "cup" },
            new SeedIngredient { Key = "muscovado-sugar", Name = "Muscovado Sugar", Category = "Sweetener", DefaultUnit = "cup" },

            // Cooking essentials
            new SeedIngredient { Key = "vegetable-oil", Name = "Vegetable Oil", Category = "Cooking Fat", DefaultUnit = "cup" },
            new SeedIngredient { Key = "olive-oil", Name = "Olive Oil", Category = "Cooking Fat", DefaultUnit = "cup" },
            new SeedIngredient { Key = "vinegar", Name = "Vinegar", Category = "Seasoning", DefaultUnit = "tbsp",
                Aliases = new() { ("suka", "fil") } },
            new SeedIngredient { Key = "lemon-juice", Name = "Lemon Juice", Category = "Seasoning", DefaultUnit = "tbsp" },
            new SeedIngredient { Key = "calamansi", Name = "Calamansi", Category = "Seasoning", DefaultUnit = "piece",
                Aliases = new() { ("calamondin", "en"), ("kalamansi", "fil") } },

            // Protein
            new SeedIngredient { Key = "chicken-breast", Name = "Chicken Breast", Category = "Protein", DefaultUnit = "lb",
                Aliases = new() { ("manok", "fil") } },
            new SeedIngredient { Key = "ground-pork", Name = "Ground Pork", Category = "Protein", DefaultUnit = "lb",
                Aliases = new() { ("giniling na baboy", "fil") } },
            new SeedIngredient { Key = "ground-beef", Name = "Ground Beef", Category = "Protein", DefaultUnit = "lb" },
            new SeedIngredient { Key = "tofu", Name = "Tofu", Category = "Protein", DefaultUnit = "block",
                Aliases = new() { ("tokwa", "fil") } },

            // Filipino staples
            new SeedIngredient { Key = "coconut-milk", Name = "Coconut Milk", Category = "Filipino Staple", DefaultUnit = "cup",
                Aliases = new() { ("gata", "fil") } },
            new SeedIngredient { Key = "fish-sauce", Name = "Fish Sauce", Category = "Filipino Staple", DefaultUnit = "tbsp",
                Aliases = new() { ("patis", "fil") } },
            new SeedIngredient { Key = "soy-sauce", Name = "Soy Sauce", Category = "Filipino Staple", DefaultUnit = "tbsp",
                Aliases = new() { ("toyo", "fil") } },
            new SeedIngredient { Key = "tamarind", Name = "Tamarind", Category = "Filipino Staple", DefaultUnit = "piece",
                Aliases = new() { ("sampalok", "fil") } },
            new SeedIngredient { Key = "shrimp-paste", Name = "Shrimp Paste", Category = "Filipino Staple", DefaultUnit = "tbsp",
                Aliases = new() { ("bagoong", "fil") } },

            // Vegetables
            new SeedIngredient { Key = "cabbage", Name = "Cabbage", Category = "Vegetable", DefaultUnit = "cup",
                Aliases = new() { ("repolyo", "fil") } },
            new SeedIngredient { Key = "carrot", Name = "Carrot", Category = "Vegetable", DefaultUnit = "piece",
                Aliases = new() { ("karot", "fil") } },
            new SeedIngredient { Key = "eggplant", Name = "Eggplant", Category = "Vegetable", DefaultUnit = "piece",
                Aliases = new() { ("talong", "fil") } },
            new SeedIngredient { Key = "bok-choy", Name = "Bok Choy", Category = "Vegetable", DefaultUnit = "cup",
                Aliases = new() { ("pechay", "fil") } },
        };

        public static readonly List<SeedSubstitution> Substitutions = new()
        {
            // --- Milk / Buttermilk ---
            new SeedSubstitution
            {
                SeedKey = "milk-to-buttermilk-sub-via-vinegar",
                OriginalIngredientKey = "buttermilk",
                Components = new()
                {
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "milk", RatioNumerator = 1, RatioDenominator = 1, OriginalUnit = "cup", SubstituteUnit = "cup" },
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "vinegar", RatioNumerator = 1, RatioDenominator = 1, OriginalUnit = "cup", SubstituteUnit = "tbsp" }
                },
                UseCases = new() { SubstitutionUseCase.Baking },
                Instructions = "Add 1 tablespoon of vinegar (or lemon juice) to 1 cup of milk. Stir and let sit 5-10 minutes until slightly curdled before using.",
                FlavorImpact = "Very close to real buttermilk; a very slight tang.",
                TextureImpact = "Thinner than cultured buttermilk but works well in batters.",
                Limitations = "Not a substitute for buttermilk in recipes relying on its live cultures (e.g. some fermented doughs).",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },
            new SeedSubstitution
            {
                SeedKey = "buttermilk-to-yogurt",
                OriginalIngredientKey = "buttermilk",
                Components = new()
                {
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "plain-yogurt", RatioNumerator = 3, RatioDenominator = 4, OriginalUnit = "cup", SubstituteUnit = "cup" }
                },
                UseCases = new() { SubstitutionUseCase.Baking },
                Instructions = "Thin 3/4 cup plain yogurt with a splash of milk or water to reach 1 cup total volume.",
                FlavorImpact = "Slightly tangier than buttermilk.",
                TextureImpact = "Thicker; thin with liquid to match buttermilk's pourable consistency.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Butter / Margarine ---
            new SeedSubstitution
            {
                SeedKey = "butter-to-margarine",
                OriginalIngredientKey = "butter",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "margarine", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.Baking, SubstitutionUseCase.Frying, SubstitutionUseCase.General },
                Instructions = "Use margarine in the same quantity as butter.",
                FlavorImpact = "Slightly less rich flavor.",
                TextureImpact = "Baked goods may be slightly less tender/flaky.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },
            new SeedSubstitution
            {
                SeedKey = "butter-to-vegetable-oil",
                OriginalIngredientKey = "butter",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "vegetable-oil", RatioNumerator = 3, RatioDenominator = 4 } },
                UseCases = new() { SubstitutionUseCase.Baking },
                Instructions = "Use about 3/4 the amount of oil as butter called for (e.g. 3/4 cup oil for 1 cup butter).",
                FlavorImpact = "Less buttery flavor.",
                TextureImpact = "Denser, moister crumb; less flaky in pastries.",
                Limitations = "Not recommended for recipes relying on creaming butter and sugar (e.g. classic cookies).",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Heavy cream ---
            new SeedSubstitution
            {
                SeedKey = "heavy-cream-to-milk-and-butter",
                OriginalIngredientKey = "heavy-cream",
                Components = new()
                {
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "milk", RatioNumerator = 3, RatioDenominator = 4, OriginalUnit = "cup", SubstituteUnit = "cup" },
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "butter", RatioNumerator = 1, RatioDenominator = 4, OriginalUnit = "cup", SubstituteUnit = "cup" }
                },
                UseCases = new() { SubstitutionUseCase.Sauces, SubstitutionUseCase.Soups, SubstitutionUseCase.Baking },
                Instructions = "Melt 1/4 cup butter and whisk into 3/4 cup milk to approximate 1 cup heavy cream.",
                FlavorImpact = "Close, slightly less rich.",
                TextureImpact = "Will not whip into stiff peaks like real heavy cream.",
                Limitations = "Not suitable for whipped cream toppings.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },
            new SeedSubstitution
            {
                SeedKey = "heavy-cream-to-evaporated-milk",
                OriginalIngredientKey = "heavy-cream",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "evaporated-milk", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.Soups, SubstitutionUseCase.Sauces },
                Instructions = "Use evaporated milk in place of heavy cream for soups and sauces (not for whipping).",
                FlavorImpact = "Slightly less rich, a bit sweeter.",
                TextureImpact = "Thinner; won't thicken sauces as much.",
                Limitations = "Cannot be whipped.",
                VerificationStatus = SubstitutionVerificationStatus.Unverified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Sour cream ---
            new SeedSubstitution
            {
                SeedKey = "sour-cream-to-yogurt",
                OriginalIngredientKey = "sour-cream",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "plain-yogurt", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.General, SubstitutionUseCase.Baking },
                Instructions = "Use plain yogurt (preferably strained/Greek-style) in the same quantity as sour cream.",
                FlavorImpact = "Very similar tang.",
                TextureImpact = "Slightly thinner unless using strained/Greek yogurt.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Evaporated / Condensed milk ---
            new SeedSubstitution
            {
                SeedKey = "evaporated-milk-to-milk",
                OriginalIngredientKey = "evaporated-milk",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "milk", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.General, SubstitutionUseCase.Baking },
                Instructions = "Use regular milk 1:1; the result will be less rich and thinner.",
                FlavorImpact = "Less rich/caramelized flavor.",
                TextureImpact = "Thinner.",
                VerificationStatus = SubstitutionVerificationStatus.Unverified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Eggs ---
            new SeedSubstitution
            {
                SeedKey = "egg-to-cornstarch-slurry",
                OriginalIngredientKey = "egg",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "cornstarch", RatioNumerator = 1, RatioDenominator = 1, OriginalUnit = "piece", SubstituteUnit = "tbsp" } },
                UseCases = new() { SubstitutionUseCase.Baking },
                Instructions = "Mix 1 tablespoon cornstarch with 3 tablespoons water to replace 1 egg as a binder.",
                FlavorImpact = "Neutral.",
                TextureImpact = "Denser results; works best as a binder, not for leavening or structure in cakes.",
                Limitations = "Not a good substitute where eggs provide significant leavening (e.g. sponge cakes, souffles).",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Flour ---
            new SeedSubstitution
            {
                SeedKey = "flour-thickener-to-cornstarch",
                OriginalIngredientKey = "all-purpose-flour",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "cornstarch", RatioNumerator = 1, RatioDenominator = 2 } },
                UseCases = new() { SubstitutionUseCase.Sauces, SubstitutionUseCase.Soups },
                Instructions = "When flour is used only as a thickener, use half the amount of cornstarch (cornstarch thickens more strongly).",
                FlavorImpact = "Neutral; cornstarch gives a glossier finish.",
                TextureImpact = "Silkier, more translucent sauce versus flour's matte, opaque thickening.",
                Limitations = "Only applies when flour is used as a thickener, not for baking structure.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Baking powder / soda ---
            new SeedSubstitution
            {
                SeedKey = "baking-powder-to-baking-soda-and-acid",
                OriginalIngredientKey = "baking-powder",
                Components = new()
                {
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "baking-soda", RatioNumerator = 1, RatioDenominator = 3, OriginalUnit = "tsp", SubstituteUnit = "tsp" },
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "vinegar", RatioNumerator = 2, RatioDenominator = 3, OriginalUnit = "tsp", SubstituteUnit = "tsp" }
                },
                UseCases = new() { SubstitutionUseCase.Baking },
                Instructions = "For 1 tsp baking powder, combine 1/3 tsp baking soda with 2/3 tsp vinegar or lemon juice; add to the batter immediately before baking.",
                FlavorImpact = "Negligible if balanced correctly.",
                TextureImpact = "Reacts immediately, so bake promptly after mixing to preserve rise.",
                Limitations = "Ratio is approximate; too much acid or soda can affect taste.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Sugars ---
            new SeedSubstitution
            {
                SeedKey = "brown-sugar-to-white-sugar-and-molasses",
                OriginalIngredientKey = "brown-sugar",
                Components = new()
                {
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "white-sugar", RatioNumerator = 1, RatioDenominator = 1 }
                },
                UseCases = new() { SubstitutionUseCase.Baking },
                Instructions = "White sugar can be used 1:1 in a pinch, though molasses (if available) added back restores the characteristic flavor and moisture.",
                FlavorImpact = "Less caramel/molasses flavor.",
                TextureImpact = "Slightly less moist baked goods.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },
            new SeedSubstitution
            {
                SeedKey = "brown-sugar-to-muscovado",
                OriginalIngredientKey = "brown-sugar",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "muscovado-sugar", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.Baking, SubstitutionUseCase.General },
                Instructions = "Use muscovado sugar in the same quantity; it is commonly available in the Philippines and similar in character to brown sugar.",
                FlavorImpact = "Deeper, more pronounced molasses flavor.",
                TextureImpact = "Slightly more moist due to higher molasses content.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "filipino-home-cooking-practice"
            },
            new SeedSubstitution
            {
                SeedKey = "white-sugar-to-honey",
                OriginalIngredientKey = "white-sugar",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "honey", RatioNumerator = 3, RatioDenominator = 4, OriginalUnit = "cup", SubstituteUnit = "cup" } },
                UseCases = new() { SubstitutionUseCase.Baking, SubstitutionUseCase.Drinks },
                Instructions = "Use about 3/4 cup honey per 1 cup sugar and reduce other liquids in the recipe slightly, since honey adds moisture.",
                FlavorImpact = "Noticeably different, honey flavor and faster browning.",
                TextureImpact = "Moister, denser baked goods; browns faster so lower oven temp by about 25°F (15°C) if baking.",
                Limitations = "Not a direct 1:1 swap; recipe adjustments needed for best results.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Oils ---
            new SeedSubstitution
            {
                SeedKey = "vegetable-oil-to-olive-oil",
                OriginalIngredientKey = "vegetable-oil",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "olive-oil", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.Frying, SubstitutionUseCase.General },
                Instructions = "Use olive oil in the same quantity for general cooking and sauteing.",
                FlavorImpact = "More distinct, slightly fruity/peppery flavor.",
                TextureImpact = "No significant change for frying/sauteing.",
                Limitations = "Not ideal for high-heat deep frying or neutral-flavored baking.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Vinegar / Lemon / Calamansi (very relevant to Filipino cooking) ---
            new SeedSubstitution
            {
                SeedKey = "vinegar-to-calamansi",
                OriginalIngredientKey = "vinegar",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "calamansi", RatioNumerator = 2, RatioDenominator = 1, OriginalUnit = "tbsp", SubstituteUnit = "piece" } },
                UseCases = new() { SubstitutionUseCase.General, SubstitutionUseCase.Sauces },
                Instructions = "Use the juice of about 2 calamansi per tablespoon of vinegar called for, adjusting to taste.",
                FlavorImpact = "Brighter, more citrus-forward sourness versus vinegar's sharper acidity.",
                TextureImpact = "No significant change.",
                Limitations = "Not a perfect match for recipes that specifically need vinegar's stronger acidity (e.g. pickling, adobo for long shelf life).",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "filipino-home-cooking-practice"
            },
            new SeedSubstitution
            {
                SeedKey = "lemon-juice-to-calamansi",
                OriginalIngredientKey = "lemon-juice",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "calamansi", RatioNumerator = 2, RatioDenominator = 1, OriginalUnit = "tbsp", SubstituteUnit = "piece" } },
                UseCases = new() { SubstitutionUseCase.General, SubstitutionUseCase.Drinks, SubstitutionUseCase.Sauces },
                Instructions = "Use juice from about 2 calamansi per tablespoon of lemon juice, adjusting to taste.",
                FlavorImpact = "Similar citrus tang, slightly milder and more floral than lemon.",
                TextureImpact = "No significant change.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "filipino-home-cooking-practice"
            },
            new SeedSubstitution
            {
                SeedKey = "calamansi-to-lemon-juice",
                OriginalIngredientKey = "calamansi",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "lemon-juice", RatioNumerator = 1, RatioDenominator = 2, OriginalUnit = "piece", SubstituteUnit = "tbsp" } },
                UseCases = new() { SubstitutionUseCase.General, SubstitutionUseCase.Drinks },
                Instructions = "Use about 1/2 tablespoon lemon juice per calamansi called for, adjusting to taste.",
                FlavorImpact = "Sharper, less floral than calamansi.",
                TextureImpact = "No significant change.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "filipino-home-cooking-practice"
            },

            // --- Protein ---
            new SeedSubstitution
            {
                SeedKey = "ground-pork-to-ground-beef",
                OriginalIngredientKey = "ground-pork",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "ground-beef", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.General, SubstitutionUseCase.Frying },
                Instructions = "Use ground beef in the same quantity; adjust seasoning slightly as beef has a stronger flavor.",
                FlavorImpact = "More pronounced, beefier flavor.",
                TextureImpact = "Can be leaner depending on cut; may need added fat/oil.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },
            new SeedSubstitution
            {
                SeedKey = "chicken-breast-to-tofu",
                OriginalIngredientKey = "chicken-breast",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "tofu", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.General, SubstitutionUseCase.Frying, SubstitutionUseCase.Soups },
                Instructions = "Use firm tofu, pressed and cubed, in roughly the same quantity. Marinate well since tofu absorbs flavor more slowly than meat.",
                FlavorImpact = "Milder; depends heavily on marinade/seasoning.",
                TextureImpact = "Softer, less fibrous texture.",
                Limitations = "Best for stir-fries, soups, and braises rather than recipes relying on meat's specific texture (e.g. grilled/fried chicken dishes).",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },

            // --- Filipino staples ---
            new SeedSubstitution
            {
                SeedKey = "fish-sauce-to-soy-sauce",
                OriginalIngredientKey = "fish-sauce",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "soy-sauce", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.General, SubstitutionUseCase.Soups, SubstitutionUseCase.Sauces },
                Instructions = "Use soy sauce in the same quantity; the dish will lose some of fish sauce's distinct umami depth.",
                FlavorImpact = "Less funky/umami depth; saltier, more one-dimensional.",
                TextureImpact = "No change.",
                Limitations = "Not suitable where fish sauce's specific flavor is central to the dish (e.g. some dipping sauces).",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "filipino-home-cooking-practice"
            },
            new SeedSubstitution
            {
                SeedKey = "coconut-milk-to-evaporated-milk",
                OriginalIngredientKey = "coconut-milk",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "evaporated-milk", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.Soups, SubstitutionUseCase.Sauces },
                Instructions = "Use evaporated milk in the same quantity for a creamy texture in savory dishes; coconut flavor will be lost.",
                FlavorImpact = "Loses the distinct coconut flavor.",
                TextureImpact = "Similarly creamy/rich.",
                Limitations = "Not suitable where coconut flavor is essential (e.g. ginataan dishes).",
                VerificationStatus = SubstitutionVerificationStatus.Unverified,
                SourceKey = "filipino-home-cooking-practice"
            },
            new SeedSubstitution
            {
                SeedKey = "tamarind-to-vinegar-and-sugar",
                OriginalIngredientKey = "tamarind",
                Components = new()
                {
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "vinegar", RatioNumerator = 1, RatioDenominator = 1 },
                    new SeedSubstitutionComponent { SubstituteIngredientKey = "brown-sugar", RatioNumerator = 1, RatioDenominator = 2 }
                },
                UseCases = new() { SubstitutionUseCase.Soups },
                Instructions = "Combine vinegar with a small amount of brown sugar to approximate tamarind's sweet-sour profile for soups like sinigang; adjust both to taste since tamarind's exact sourness varies.",
                FlavorImpact = "Less complex, more one-dimensional sourness.",
                TextureImpact = "No significant change.",
                Limitations = "Approximate only; many cooks prefer commercial sinigang mix as a closer substitute, which is outside this ingredient-based dataset.",
                VerificationStatus = SubstitutionVerificationStatus.Unverified,
                SourceKey = "filipino-home-cooking-practice"
            },

            // --- Vegetables ---
            new SeedSubstitution
            {
                SeedKey = "bok-choy-to-cabbage",
                OriginalIngredientKey = "bok-choy",
                Components = new() { new SeedSubstitutionComponent { SubstituteIngredientKey = "cabbage", RatioNumerator = 1, RatioDenominator = 1 } },
                UseCases = new() { SubstitutionUseCase.Soups, SubstitutionUseCase.Frying, SubstitutionUseCase.General },
                Instructions = "Use shredded cabbage in the same quantity; add a few minutes earlier than bok choy since cabbage takes slightly longer to soften.",
                FlavorImpact = "Milder, less peppery.",
                TextureImpact = "Slightly crunchier when first added; softens with longer cooking.",
                VerificationStatus = SubstitutionVerificationStatus.Verified,
                SourceKey = "general-culinary-knowledge"
            },
        };
    }
}
