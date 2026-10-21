using Microsoft.Extensions.Logging;
using NutriSubs.Data;
using NutriSubs.Data.Repositories;
using NutriSubs.Data.Seed;
using NutriSubs.Services;

namespace NutriSubs
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Substitution database: a single shared connection/initializer per app session,
            // with repositories and the lookup service resolved per-use through DI.
            builder.Services.AddSingleton<SubstitutionDatabase>();
            builder.Services.AddSingleton<DatabaseInitializer>();
            builder.Services.AddTransient<DatabaseSeeder>();
            builder.Services.AddTransient<IIngredientRepository, IngredientRepository>();
            builder.Services.AddTransient<ISubstitutionRepository, SubstitutionRepository>();
            builder.Services.AddTransient<ISubstitutionLookupService, SubstitutionLookupService>();

            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
