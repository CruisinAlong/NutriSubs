using NutriSubs.Models;

using NutriSubs.Services;

namespace NutriSubs
{
    public partial class RecipePage : ContentPage
    {
        private readonly Recipe _recipe;
        private int _currentServings;

        // Event exposed for another team member to hook up the substitution feature later.
        // Example usage: recipePage.FindSubstitutesRequested += (s, recipe) => { /* navigate to substitution screen */ };
        public event EventHandler<Recipe>? FindSubstitutesRequested;

        // Opens a sample recipe by id (existing behavior, unchanged).
        public RecipePage(int recipeId)
            : this(SampleRecipes.GetById(recipeId)
                   ?? throw new ArgumentException($"No sample recipe found with id {recipeId}", nameof(recipeId)))
        {
        }

        // Opens a recipe object directly. Used for recipes imported from a URL,
        // which are displayed using the exact same screen/layout as sample recipes.
        public RecipePage(Recipe recipe)
        {
            InitializeComponent();

            _recipe = recipe ?? throw new ArgumentNullException(nameof(recipe));
            _currentServings = _recipe.Servings > 0 ? _recipe.Servings : 1;

            NameLabel.Text = _recipe.Name;
            DescriptionLabel.Text = _recipe.Description;
            PrepTimeLabel.Text = $"Prep: {_recipe.PrepTimeMinutes} min";
            CookTimeLabel.Text = $"Cook: {_recipe.CookTimeMinutes} min";

            if (!string.IsNullOrWhiteSpace(_recipe.SourceUrl))
            {
                SourceUrlLabel.Text = $"Source: {_recipe.SourceUrl}";
                SourceUrlLabel.IsVisible = true;
            }

            IngredientsCollectionView.ItemsSource = _recipe.Ingredients;
            InstructionsCollectionView.ItemsSource = _recipe.Instructions;

            UpdateServingsLabel();
        }

        private void UpdateServingsLabel()
        {
            ServingsLabel.Text = $"{_currentServings} servings";
        }

        private void OnIncreaseServingsClicked(object? sender, EventArgs e)
        {
            _currentServings++;
            UpdateServingsLabel();
        }

        private void OnDecreaseServingsClicked(object? sender, EventArgs e)
        {
            if (_currentServings > 1)
                _currentServings--;

            UpdateServingsLabel();
        }

        private async void OnFindSubstitutesClicked(object? sender, EventArgs e)
        {
            FindSubstitutesRequested?.Invoke(this, _recipe);

            if (_recipe.Ingredients.Count == 0)
            {
                await DisplayAlert("Find Substitutes", "This recipe has no ingredients listed.", "OK");
                return;
            }

            var choice = await DisplayActionSheet(
                "Find substitutes for which ingredient?",
                "Cancel",
                null,
                _recipe.Ingredients.ToArray());

            if (string.IsNullOrWhiteSpace(choice) || choice == "Cancel")
                return;

            var lookupService = Handler?.MauiContext?.Services.GetService<ISubstitutionLookupService>();

            if (lookupService is null)
            {
                await DisplayAlert("Find Substitutes", "The substitution feature is unavailable right now.", "OK");
                return;
            }

            await Navigation.PushAsync(new SubstitutesPage(lookupService, choice));
        }
    }
}
