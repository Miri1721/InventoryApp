using InventoryApp.Mobile.Services;
using Microsoft.Extensions.Logging;

namespace InventoryApp.Mobile
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

            builder.Services.AddSingleton<AuthApiService>();
            builder.Services.AddSingleton<CategoryApiService>();
            builder.Services.AddSingleton<ItemApiService>();
            builder.Services.AddSingleton<StockTransactionApiService>();
            builder.Services.AddSingleton<ReportApiService>();
            builder.Services.AddSingleton<OrganizationApiService>();
            builder.Services.AddSingleton<UserApiService>();

            return builder.Build();
        }
    }
}