using NutriSubs.Models;

namespace NutriSubs
{
    public partial class MainPage : ContentPage
    {
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

            // Recipe importing/scraping is a planned feature and is not implemented yet.
            await DisplayAlert("Import Recipe", "Recipe importing from a URL is a planned feature and isn't available yet.", "OK");
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
