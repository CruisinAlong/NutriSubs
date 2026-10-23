# NutriSubs

NutriSubs is a .NET MAUI mobile app that helps you cook with what you have. Browse sample Filipino and everyday recipes, then look up budget-friendly, verified substitutes for ingredients you don't have on hand — including common Filipino ingredient names (e.g. *calamansi*, *patis*, *gata*, *talong*).

## ?? Download the APK

Pre-built Android APKs are published under the **[Releases](../../releases)** page of this repository.

1. Go to the [Releases](../../releases) page.
2. Download the latest `NutriSubs.apk` from the **Assets** section of the newest release.
3. Transfer it to your Android phone (or download directly on the phone) and install it.

> If you don't see a `Releases` entry yet, the project may not have a tagged release published. See [Installation](#-installation) below for building it yourself instead.

## ?? Installation

### Option A — Install the pre-built APK (recommended for most users)

1. On your Android phone, enable **"Install unknown apps"** / **"Install from unknown sources"** for the browser or file manager you'll use (Settings ? Apps ? Special access ? Install unknown apps).
2. Download `NutriSubs.apk` from the [Releases](../../releases) page directly on your phone, or transfer it over from your PC (USB, email, cloud drive, etc.).
3. Tap the downloaded `.apk` file and follow the prompts to install.
4. Open **NutriSubs** from your app drawer.

### Option B — Build it yourself from source

Requirements:
- Visual Studio 2022 (17.14+) with the **.NET Multi-platform App UI development** workload, or the .NET 9 SDK + MAUI workload via CLI (`dotnet workload install maui`)
- .NET 9 SDK

Steps:
git clone https://github.com/CruisinAlong/NutriSubs.git
cd NutriSubs\NutriSubs\NutriSubs
dotnet build -f net9.0-android -c Release
The signed APK will be produced at:
NutriSubs\NutriSubs\bin\Release\net9.0-android\com.companyname.nutrisubs-Signed.apk

Alternatively, open `NutriSubs.sln` in Visual Studio, select the `net9.0-android` target framework and an Android emulator/device, and press **Run**.

## ? What you can do

- **Browse sample recipes** — A curated set of everyday/Filipino recipes (e.g. dishes using eggplant, mung beans, garlic rice) with ingredients, instructions, prep/cook time, and adjustable serving sizes.
- **Find ingredient substitutes** — From a recipe's ingredient list, select any ingredient to look up verified substitutes, complete with:
  - Substitution ratio (e.g. "3/4 cup plain yogurt for 1 cup sour cream")
  - Flavor and texture impact notes
  - Known limitations (e.g. "not suitable for whipped toppings")
  - Source/reference attribution
- **Smart ingredient recognition** — Understands free-form recipe text such as quantities, units, and common abbreviations (e.g. `"3 tablespoons calamansi"`, `"1/2 cup all-purpose flour"`), and recognizes English, Filipino, and common alias names for the same ingredient (e.g. *gata* ? Coconut Milk, *patis* ? Fish Sauce).
- **Manual fallback search** — If an ingredient isn't automatically recognized, you can manually type the ingredient name to search again, so you're never stuck.
- **Distinguishes "not recognized" vs. "no substitutes available"** — The app clearly tells you whether an ingredient simply isn't in the database yet, or if it's recognized but doesn't have a verified substitute.

## ?? Substitutable ingredients

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

## ??? Tech stack

- **.NET MAUI** targeting **.NET 9** (Android, iOS, Mac Catalyst, Windows)
- **SQLite** (via `sqlite-net-pcl`) for the local, offline substitution database
- Seed data idempotently loaded/upserted on first run — no internet connection required

## ?? License

See repository license/terms as applicable.

This version maintains the original structure while ensuring clarity and coherence throughout the document. Each section is clearly defined, and the flow of information is logical and easy to follow.
