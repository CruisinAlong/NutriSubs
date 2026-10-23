using NutriSubs.Services;

namespace NutriSubs.Tests;

public class IngredientNameMatcherTests
{
    [Theory]
    [InlineData("calamansi", "calamansi juice", true)]
    [InlineData("calamansi juice", "calamansi", true)]
    [InlineData("coconut milk", "coconut cream", false)]
    [InlineData("rice vinegar", "white vinegar", false)]
    [InlineData("ground black pepper", "black pepper sauce", false)]
    [InlineData("black pepper", "ground black pepper", false)]
    public void Matches_Only_Whole_Word_Prefix_Relationships(string normalized, string candidate, bool expectedMatch)
    {
        var normalizedWords = normalized.Split(' ');
        var candidateWords = candidate.Split(' ');

        var result = IngredientNameMatcher.IsConstrainedPartialMatch(normalizedWords, candidateWords);

        Assert.Equal(expectedMatch, result);
    }
}
