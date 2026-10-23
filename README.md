# NutriSubs

NutriSubs is a .NET MAUI mobile app that helps you cook with what you have. Browse sample Filipino and everyday recipes, then look up budget-friendly, verified substitutes for ingredients you don't have on hand — including common Filipino ingredient names (e.g. *calamansi*, *patis*, *gata*, *talong*).

## Download the APK

Pre-built Android APKs are published under the **[Releases](../../releases)** page of this repository.

1. Go to the [Releases](../../releases) page.
2. Download the latest `NutriSubs.apk` from the **Assets** section of the newest release.
3. Transfer it to your Android phone (or download directly on the phone) and install it.

##  What you can do

- **Browse sample recipes** — A curated set of everyday/Filipino recipes (e.g. dishes using eggplant, mung beans, garlic rice) with ingredients, instructions, prep/cook time, and adjustable serving sizes.
- **Find ingredient substitutes** — From a recipe's ingredient list, select any ingredient to look up verified substitutes, complete with:
  - Substitution ratio (e.g. "3/4 cup plain yogurt for 1 cup sour cream")
  - Flavor and texture impact notes
  - Known limitations (e.g. "not suitable for whipped toppings")
  - Source/reference attribution
- **Smart ingredient recognition** — Understands free-form recipe text such as quantities, units, and common abbreviations (e.g. `"3 tablespoons calamansi"`, `"1/2 cup all-purpose flour"`), and recognizes English, Filipino, and common alias names for the same ingredient (e.g. *gata* ? Coconut Milk, *patis* ? Fish Sauce).
- **Manual fallback search** — If an ingredient isn't automatically recognized, you can manually type the ingredient name to search again, so you're never stuck.
- **Distinguishes "not recognized" vs. "no substitutes available"** — The app clearly tells you whether an ingredient simply isn't in the database yet, or if it's recognized but doesn't have a verified substitute.

## Substitutable ingredients

The app ships with a curated seed dataset of **~35 ingredients** across common categories, each with one or more verified substitution options. Supported ingredients (and their common aliases) include:

### Dairy
Milk · Buttermilk · Butter · Margarine · Heavy Cream (*heavy whipping cream*) · Plain Yogurt · Sour Cream · Evaporated Milk · Condensed Milk

### Baking
Egg (*itlog*) · All-Purpose Flour (*harina*) · Baking Powder · Baking Soda · Cornstarch · Vanilla Extract

### Sweeteners
White Sugar (*asukal*) · Brown Sugar · Honey · Muscovado Sugar

### Cooking Essentials & Seasoning
Vegetable Oil · Olive Oil · Vinegar (*suka*) · Lemon Juice · Calamansi (*calamondin*, *kalamansi*)

### Protein
Chicken Breast (*manok*) · Ground Pork (*giniling na baboy*) · Ground Beef · Tofu (*tokwa*)

### Filipino Staples
Coconut Milk (*gata*) · Fish Sauce (*patis*) · Soy Sauce (*toyo*) · Tamarind (*sampalok*) · Shrimp Paste (*bagoong*)

### Vegetables
Cabbage (*repolyo*) · Carrot (*karot*) · Eggplant (*talong*) · Bok Choy (*pechay*)

Each ingredient may have multiple substitution recipes depending on use case (Baking, Frying, Sauces, Soups, Drinks, or General), along with notes on flavor/texture impact and limitations. The ingredient and substitution list is actively maintained and expected to grow over time — if your ingredient isn't recognized yet, try the in-app manual search, as more entries may already cover it under a different name or spelling.

## Tech stack

- **.NET MAUI** targeting **.NET 9** (Android, iOS, Mac Catalyst, Windows)
- **SQLite** (via `sqlite-net-pcl`) for the local, offline substitution database
- Seed data idempotently loaded/upserted on first run — no internet connection required

