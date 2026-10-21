using NutriSubs.Data;

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
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}