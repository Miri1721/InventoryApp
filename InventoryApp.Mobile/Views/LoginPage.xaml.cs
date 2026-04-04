using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthApiService _authApiService;
    private readonly CategoryApiService _categoryApiService;
    private readonly ItemApiService _itemApiService;
    private readonly StockTransactionApiService _stockTransactionApiService;
    private readonly ReportApiService _reportApiService;
    private readonly OrganizationApiService _organizationApiService;

    public LoginPage(AuthApiService authApiService,
                     CategoryApiService categoryApiService,
                     ItemApiService itemApiService,
                     StockTransactionApiService stockTransactionApiService,
                     ReportApiService reportApiService,
                     OrganizationApiService organizationApiService)
    {
        InitializeComponent();
        _authApiService = authApiService;
        _categoryApiService = categoryApiService;
        _itemApiService = itemApiService;
        _stockTransactionApiService = stockTransactionApiService;
        _reportApiService = reportApiService;
        _organizationApiService = organizationApiService;
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

            AppSession.FullName = response.FullName ?? string.Empty;
            AppSession.Email = response.Email ?? string.Empty;
            AppSession.Role = response.Role ?? string.Empty;
            AppSession.OrganizationName = response.OrganizationName ?? string.Empty;
            AppSession.OrganizationId = response.OrganizationId ?? Guid.Empty;
            AppSession.OrganizationType = response.OrganizationType ?? string.Empty;

            await Navigation.PushAsync(new DashboardPage(
                                             _authApiService,
                                             _categoryApiService,
                                             _itemApiService,
                                             _stockTransactionApiService,
                                             _reportApiService,
                                             _organizationApiService));
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Login failed: {ex.Message}";
        }
    }

    private async void OnRegisterLabelTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_authApiService, _organizationApiService));
    }
}