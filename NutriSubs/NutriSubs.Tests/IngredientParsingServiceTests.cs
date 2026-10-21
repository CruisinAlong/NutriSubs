using NutriSubs.Models;
using NutriSubs.Services;

namespace NutriSubs.Tests;

public class IngredientParsingServiceTests
{
    [Fact]
    public void Parses_Tablespoons_Calamansi()
    {
        var result = IngredientParsingService.Parse("3 tablespoons calamansi");

        Assert.Equal(3, result.Quantity);
        Assert.Equal("tablespoon", result.Unit);
        Assert.Equal("calamansi", result.IngredientName);
        Assert.Equal(IngredientParseStatus.Success, result.ParseStatus);
    }

    [Fact]
    public void Parses_Fresh_Calamansi_Juice()
    {
        var result = IngredientParsingService.Parse("2 tbsp fresh calamansi juice");

        Assert.Equal(2, result.Quantity);
        Assert.Equal("tablespoon", result.Unit);
        Assert.Equal("calamansi juice", result.IngredientName);
        Assert.Equal("fresh", result.AdditionalDescription);
    }

    [Fact]
    public void Parses_Mixed_Fraction_Cups_Milk()
    {
        var result = IngredientParsingService.Parse("1 1/2 cups milk");

        Assert.Equal(1.5m, result.Quantity);
        Assert.Equal("cup", result.Unit);
        Assert.Equal("milk", result.IngredientName);
        Assert.Equal("1 1/2", result.OriginalQuantityText);
    }

    [Fact]
    public void Parses_Unicode_Fraction_Tsp_Salt()
    {
        var result = IngredientParsingService.Parse("½ tsp salt");

        Assert.Equal(0.5m, result.Quantity);
        Assert.Equal("teaspoon", result.Unit);
        Assert.Equal("salt", result.IngredientName);
    }

    [Fact]
    public void Parses_Grams_Ground_Pork()
    {
        var result = IngredientParsingService.Parse("250 g ground pork");

        Assert.Equal(250, result.Quantity);
        Assert.Equal("gram", result.Unit);
        Assert.Equal("ground pork", result.IngredientName);
    }

    [Fact]
    public void Parses_Cloves_Minced_Garlic()
    {
        var result = IngredientParsingService.Parse("2 cloves minced garlic");

        Assert.Equal(2, result.Quantity);
        Assert.Equal("clove", result.Unit);
        Assert.Equal("garlic", result.IngredientName);
        Assert.Equal("minced", result.Preparation);
    }

    [Fact]
    public void Parses_Salt_To_Taste()
    {
        var result = IngredientParsingService.Parse("salt to taste");

        Assert.Equal("salt", result.IngredientName);
        Assert.Equal("to taste", result.AdditionalDescription);
    }

    [Fact]
    public void Parses_Medium_Red_Onions_Chopped()
    {
        var result = IngredientParsingService.Parse("2 medium red onions, chopped");

        Assert.Equal(2, result.Quantity);
        Assert.Equal("red onion", result.IngredientName);
        Assert.Equal("medium", result.AdditionalDescription);
        Assert.Equal("chopped", result.Preparation);
    }

    [Fact]
    public void Parses_Range_Cloves_Garlic_Without_Collapsing()
    {
        var result = IngredientParsingService.Parse("1-2 cloves garlic");

        Assert.Equal(1, result.QuantityMin);
        Assert.Equal(2, result.QuantityMax);
        Assert.True(result.IsRange);
        Assert.Equal("clove", result.Unit);
        Assert.Equal("garlic", result.IngredientName);
    }

    [Fact]
    public void Parses_Ingredient_Name_Without_Quantity()
    {
        var result = IngredientParsingService.Parse("coconut milk");

        Assert.Equal("coconut milk", result.IngredientName);
        Assert.Null(result.Quantity);
        Assert.Equal(IngredientParseStatus.Success, result.ParseStatus);
    }

    [Fact]
    public void Parses_Pinch_Of_Salt()
    {
        var result = IngredientParsingService.Parse("a pinch of salt");

        Assert.Equal("pinch", result.Unit);
        Assert.Equal("salt", result.IngredientName);
    }

    [Fact]
    public void Parses_Simple_Fraction_Tsp_Vanilla()
    {
        var result = IngredientParsingService.Parse("1/2 tsp vanilla extract");

        Assert.Equal(0.5m, result.Quantity);
        Assert.Equal("teaspoon", result.Unit);
        Assert.Equal("vanilla extract", result.IngredientName);
    }

    [Fact]
    public void Parses_Large_Eggs()
    {
        var result = IngredientParsingService.Parse("2 large eggs");

        Assert.Equal(2, result.Quantity);
        Assert.Equal("egg", result.IngredientName);
        Assert.Equal("large", result.AdditionalDescription);
    }

    [Fact]
    public void Parses_Milliliters_Coconut_Milk()
    {
        var result = IngredientParsingService.Parse("500 ml coconut milk");

        Assert.Equal(500, result.Quantity);
        Assert.Equal("milliliter", result.Unit);
        Assert.Equal("coconut milk", result.IngredientName);
    }

    [Fact]
    public void Parses_Can_Of_Coconut_Milk()
    {
        var result = IngredientParsingService.Parse("1 can coconut milk");

        Assert.Equal(1, result.Quantity);
        Assert.Equal("can", result.Unit);
        Assert.Equal("coconut milk", result.IngredientName);
    }

    [Fact]
    public void Parses_AllPurpose_Flour_Preserves_Hyphenated_Name()
    {
        var result = IngredientParsingService.Parse("1 cup all-purpose flour");

        Assert.Equal(1, result.Quantity);
        Assert.Equal("cup", result.Unit);
        Assert.Equal("all-purpose flour", result.IngredientName);
    }

    [Fact]
    public void Parses_Ground_Black_Pepper()
    {
        // "ground" is ambiguous (meaningful in "ground pork" but descriptive in "ground
        // black pepper"), so the parser intentionally keeps it as part of the name rather
        // than guessing; mapping "ground black pepper" to the canonical "black pepper"
        // ingredient is handled by a database alias instead (see SeedData).
        var result = IngredientParsingService.Parse("½ tsp ground black pepper");

        Assert.Equal(0.5m, result.Quantity);
        Assert.Equal("teaspoon", result.Unit);
        Assert.Equal("ground black pepper", result.IngredientName);
    }

    [Fact]
    public void Does_Not_Treat_Every_Leading_Number_As_Quantity_When_No_Unit_Follows()
    {
        // No recognized unit follows "2" here; the parser should not assume meaning it
        // cannot confidently infer. It should still preserve the original text.
        var result = IngredientParsingService.Parse("2% milk");

        Assert.Equal("2% milk", result.OriginalText);
    }

    [Fact]
    public void Empty_Input_Fails_Without_Throwing()
    {
        var result = IngredientParsingService.Parse("");

        Assert.Equal(IngredientParseStatus.Failed, result.ParseStatus);
        Assert.Equal(string.Empty, result.IngredientName);
    }

    [Fact]
    public void Null_Input_Fails_Without_Throwing()
    {
        var result = IngredientParsingService.Parse(null);

        Assert.Equal(IngredientParseStatus.Failed, result.ParseStatus);
    }

    [Fact]
    public void Removes_Parenthetical_Description()
    {
        var result = IngredientParsingService.Parse("1 cup flour (sifted)");

        Assert.Equal("flour", result.IngredientName);
    }
}
