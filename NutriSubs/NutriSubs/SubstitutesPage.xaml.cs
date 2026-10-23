using NutriSubs.Data.Entities;
using NutriSubs.Services;

namespace NutriSubs
{
    public partial class SubstitutesPage : ContentPage
    {
        private readonly ISubstitutionLookupService _lookupService;
        private string _ingredientText;

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

        private async void OnManualSearchClicked(object? sender, EventArgs e)
        {
            var manualText = ManualIngredientEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(manualText))
                return;

            _ingredientText = manualText;
            Title = $"Substitutes for {_ingredientText}";
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            EmptyStateLayout.IsVisible = false;
            ManualEntryLayout.IsVisible = false;
            ResultsCollectionView.IsVisible = false;

            try
            {
                var result = await _lookupService.FindSubstitutesAsync(_ingredientText);

                if (!result.IngredientFound)
                {
                    // Automatic recognition failed: offer the manual-entry fallback so the
                    // user isn't stuck, rather than just reporting the ingredient unrecognized.
                    EmptyStateLabel.Text = $"We don't recognize \"{_ingredientText}\" yet. Try typing it a different way below.";
                    EmptyStateLayout.IsVisible = true;
                    ManualEntryLayout.IsVisible = true;
                    return;
                }

                if (result.Options.Count == 0)
                {
                    EmptyStateLabel.Text = $"No verified substitutes are available for {result.ResolvedIngredientName} yet.";
                    EmptyStateLayout.IsVisible = true;
                    return;
                }

                ResultsCollectionView.ItemsSource = result.Options.Select(ToDisplayItem).ToList();
                ResultsCollectionView.IsVisible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutriSubs] Substitution lookup failed: {ex}");
                EmptyStateLabel.Text = "Something went wrong while looking up substitutes. Please try again later.";
                EmptyStateLayout.IsVisible = true;
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
