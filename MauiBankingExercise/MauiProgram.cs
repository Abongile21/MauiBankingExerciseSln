using MauiBankingExercise.Services;
using MauiBankingExercise.ViewModels;
using MauiBankingExercise.Views;
using Microsoft.Extensions.Logging;

namespace MauiBankingExercise
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
           
            //Customers
            builder.Services.AddSingleton<CustomersListViewModel>();
            builder.Services.AddTransient<CustomersListView>();
            //Customer
            builder.Services.AddSingleton<CustomerViewModel>();
            builder.Services.AddTransient<CustomerView>();

            //Database Service
            builder.Services.AddSingleton<BankingDatabaseService>();
                



            return builder.Build();
        }
    }
}
