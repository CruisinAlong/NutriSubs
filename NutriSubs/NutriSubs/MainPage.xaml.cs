using NutriSubs.Models;
using NutriSubs.Services;

namespace NutriSubs
{
    public partial class MainPage : ContentPage
    {
        private readonly RecipeImportService _recipeImportService = new();

        public MainPage()
        {
            InitializeComponent();
            RecipesCollectionView.ItemsSource = SampleRecipes.All;
        }

        private async void OnImportRecipeClicked(object? sender, EventArgs e)
        {
            var url = RecipeUrlEntry.Text;

            if (string.IsNullOrWhiteSpace(url))
            {
                await DisplayAlert("Import Recipe", "Please paste a recipe URL before importing.", "OK");
                return;
            }

            ImportRecipeButton.IsEnabled = false;
            ImportRecipeButton.Text = "Importing...";

            try
            {
                var recipe = await _recipeImportService.ImportFromUrlAsync(url);
                await Navigation.PushAsync(new RecipePage(recipe));
            }
            catch (RecipeImportException ex)
            {
                await DisplayAlert("Import Recipe", ex.Message, "OK");
            }
            catch (Exception)
            {
                // Catch-all so an unexpected error never crashes the app during import.
                await DisplayAlert("Import Recipe", "Something went wrong while importing this recipe. Please try a different URL.", "OK");
            }
            finally
            {
                ImportRecipeButton.IsEnabled = true;
                ImportRecipeButton.Text = "Import Recipe";
            }
        }

        private async void OnViewRecipeClicked(object? sender, EventArgs e)
        {
            if (sender is Button { CommandParameter: int recipeId })
            {
                await Navigation.PushAsync(new RecipePage(recipeId));
            }
        }

        private void OnHomeClicked(object? sender, EventArgs e)
        {
            // Already on the Home screen; no-op.
        }

        private async void OnRecipesClicked(object? sender, EventArgs e)
        {
            // "Recipes" browsing is the sample recipe list already shown on this screen.
            // Scroll/focus behavior can be added later; for now just confirm to the user.
            await DisplayAlert("Recipes", "Browse the sample recipes below.", "OK");
        }
    }
}
