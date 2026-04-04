using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    private readonly AuthApiService _authApiService;
    private readonly OrganizationApiService _organizationApiService;

    public RegisterPage(AuthApiService authApiService, OrganizationApiService organizationApiService)
    {
        InitializeComponent();
        _authApiService = authApiService;
        _organizationApiService = organizationApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var organizations = await _organizationApiService.GetAllAsync();
            ExistingOrganizationPicker.ItemsSource = organizations;
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load organizations: {ex.Message}";
        }
    }

    private void OnOrganizationModeChanged(object sender, CheckedChangedEventArgs e)
    {
        bool isExisting = ExistingOrganizationRadioButton.IsChecked;

        ExistingOrganizationSection.IsVisible = isExisting;
        NewOrganizationSection.IsVisible = !isExisting;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        try
        {
            bool isExistingOrganization = ExistingOrganizationRadioButton.IsChecked;

            Guid? existingOrganizationId = null;
            string organizationName = string.Empty;
            string organizationType = string.Empty;

            if (isExistingOrganization)
            {
                if (ExistingOrganizationPicker.SelectedItem is not OrganizationModel selectedOrganization)
                {
                    MessageLabel.Text = "Please choose an existing organization.";
                    return;
                }

                existingOrganizationId = selectedOrganization.OrganizationId;
            }
            else
            {
                organizationName = OrganizationNameEntry.Text?.Trim() ?? string.Empty;
                organizationType = OrganizationTypePicker.SelectedItem?.ToString() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(organizationName))
                {
                    MessageLabel.Text = "Organization name is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(organizationType))
                {
                    MessageLabel.Text = "Organization type is required.";
                    return;
                }
            }

            var request = new RegisterRequest
            {
                FirstName = FirstNameEntry.Text?.Trim() ?? string.Empty,
                LastName = LastNameEntry.Text?.Trim() ?? string.Empty,
                IdNumber = IdNumberEntry.Text?.Trim() ?? string.Empty,
                Email = EmailEntry.Text?.Trim() ?? string.Empty,
                IsExistingOrganization = isExistingOrganization,
                ExistingOrganizationId = existingOrganizationId,
                OrganizationName = organizationName,
                OrganizationType = organizationType,
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