using NutriSubs.Models;

namespace NutriSubs
{
    public partial class RecipePage : ContentPage
    {
        private readonly Recipe _recipe;
        private int _currentServings;

        // Event exposed for another team member to hook up the substitution feature later.
        // Example usage: recipePage.FindSubstitutesRequested += (s, recipe) => { /* navigate to substitution screen */ };
        public event EventHandler<Recipe>? FindSubstitutesRequested;

        public RecipePage(int recipeId)
        {
            InitializeComponent();

            _recipe = SampleRecipes.GetById(recipeId)
                       ?? throw new ArgumentException($"No sample recipe found with id {recipeId}", nameof(recipeId));

            _currentServings = _recipe.Servings;

            NameLabel.Text = _recipe.Name;
            DescriptionLabel.Text = _recipe.Description;
            PrepTimeLabel.Text = $"Prep: {_recipe.PrepTimeMinutes} min";
            CookTimeLabel.Text = $"Cook: {_recipe.CookTimeMinutes} min";

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
            // Placeholder: the substitution engine and screen are owned by another team member.
            FindSubstitutesRequested?.Invoke(this, _recipe);

            await DisplayAlert(
                "Find Substitutes",
                "Ingredient substitution functionality will be connected here later.",
                "OK");
        }
    }
}
