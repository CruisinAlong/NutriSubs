using NutriSubs.Data;
using NutriSubs.Services;

namespace NutriSubs
{
    public partial class App : Application
    {
        public App(DatabaseInitializer databaseInitializer)
        {
            InitializeComponent();

            // Fire-and-forget: schema creation/migration/seeding runs once in the background
            // without blocking app startup or the UI thread. Failures are handled internally
            // by DatabaseInitializer (logged, feature degrades gracefully).
            _ = databaseInitializer.InitializeAsync();

#if DEBUG
            // Lightweight diagnostic self-check (Debug output only) covering ingredient
            // text normalization and matching against the seed data, e.g. "3 tablespoons
            // calamansi" -> "calamansi". Safe to run unconditionally; it does not touch
            // the database and has no effect on the UI.
            IngredientRecognitionSelfCheck.RunAndLog();
#endif
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}