using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    private readonly AuthApiService _authApiService;

    public RegisterPage(AuthApiService authApiService)
    {
        InitializeComponent();
        _authApiService = authApiService;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        try
        {
            var request = new RegisterRequest
            {
                FirstName = FirstNameEntry.Text?.Trim() ?? string.Empty,
                LastName = LastNameEntry.Text?.Trim() ?? string.Empty,
                IdNumber = IdNumberEntry.Text?.Trim() ?? string.Empty,
                Email = EmailEntry.Text?.Trim() ?? string.Empty,
                OrganizationName = OrganizationNameEntry.Text?.Trim() ?? string.Empty,
                OrganizationType = OrganizationTypePicker.SelectedItem?.ToString() ?? string.Empty,
                Password = PasswordEntry.Text ?? string.Empty,
                ConfirmPassword = ConfirmPasswordEntry.Text ?? string.Empty
            };

            var response = await _authApiService.RegisterAsync(request);

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

            await DisplayAlert("Success", response.Message, "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Registration failed: {ex.Message}";
        }
    }
}