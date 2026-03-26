using InventoryApp.Mobile.Services;
using InventoryApp.Mobile.Views;

namespace InventoryApp.Mobile;

public partial class App : Application
{
    public App(
    AuthApiService authApiService,
    CategoryApiService categoryApiService,
    ItemApiService itemApiService)
    {
        InitializeComponent();

        MainPage = new NavigationPage(
            new LoginPage(authApiService, categoryApiService, itemApiService));
    }
}