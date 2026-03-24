using InventoryApp.Mobile.Services;
using InventoryApp.Mobile.Views;

namespace InventoryApp.Mobile;

public partial class App : Application
{
    public App(AuthApiService authApiService)
    {
        InitializeComponent();

        MainPage = new NavigationPage(new LoginPage(authApiService));
    }
}