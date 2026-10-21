using NutriSubs.Data.Entities;
using NutriSubs.Services;

namespace NutriSubs
{
    public partial class SubstitutesPage : ContentPage
    {
        private readonly ISubstitutionLookupService _lookupService;
        private readonly string _ingredientText;

        public SubstitutesPage(ISubstitutionLookupService lookupService, string ingredientText)
        {
            InitializeComponent();
            _lookupService = lookupService;
            _ingredientText = ingredientText;
            Title = $"Substitutes for {ingredientText}";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            EmptyStateLabel.IsVisible = false;
            ResultsCollectionView.IsVisible = false;

            try
            {
                var result = await _lookupService.FindSubstitutesAsync(_ingredientText);

                if (!result.IngredientFound)
                {
                    EmptyStateLabel.Text = $"We don't recognize \"{_ingredientText}\" yet. Try a different ingredient name.";
                    EmptyStateLabel.IsVisible = true;
                    return;
                }

                if (result.Options.Count == 0)
                {
                    EmptyStateLabel.Text = $"No verified substitutes are available for {result.ResolvedIngredientName} yet.";
                    EmptyStateLabel.IsVisible = true;
                    return;
                }

                ResultsCollectionView.ItemsSource = result.Options.Select(ToDisplayItem).ToList();
                ResultsCollectionView.IsVisible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutriSubs] Substitution lookup failed: {ex}");
                EmptyStateLabel.Text = "Something went wrong while looking up substitutes. Please try again later.";
                EmptyStateLabel.IsVisible = true;
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private static SubstitutionDisplayItem ToDisplayItem(SubstitutionOption option)
        {
            var replacementText = string.Join(" + ", option.Components.Select(c =>
                $"{c.RatioNumerator}/{c.RatioDenominator} {c.SubstituteUnit ?? string.Empty} {c.IngredientName}".Trim()));

            return new SubstitutionDisplayItem
            {
                ReplacementText = replacementText,
                Instructions = option.Instructions,
                FlavorImpact = option.FlavorImpact,
                TextureImpact = option.TextureImpact,
                Limitations = option.Limitations,
                SourceName = option.SourceName
            };
        }

        private class SubstitutionDisplayItem
        {
            public string ReplacementText { get; set; } = string.Empty;
            public string? Instructions { get; set; }
            public string? FlavorImpact { get; set; }
            public string? TextureImpact { get; set; }
            public string? Limitations { get; set; }
            public string? SourceName { get; set; }
        }
    }
}
