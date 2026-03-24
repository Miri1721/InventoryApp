using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthApiService _authApiService;

    public LoginPage(AuthApiService authApiService)
    {
        InitializeComponent();
        _authApiService = authApiService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        try
        {
            var request = new LoginRequest
            {
                Email = EmailEntry.Text?.Trim() ?? string.Empty,
                Password = PasswordEntry.Text ?? string.Empty
            };

            var response = await _authApiService.LoginAsync(request);

            if (response == null)
            {
                MessageLabel.Text = "No response received from server.";
                return;
            }

            if (!response.Success)
            {
                MessageLabel.Text = response.Message;
                return;
            }

            await Navigation.PushAsync(new DashboardPage(response));
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Login failed: {ex.Message}";
        }
    }

    private async void OnGoToRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_authApiService));
    }
}