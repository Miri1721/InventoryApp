using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class DashboardPage : ContentPage
{
    private readonly CategoryApiService _categoryApiService;
    private readonly ItemApiService _itemApiService;
    private readonly StockTransactionApiService _stockTransactionApiService;
    private readonly ReportApiService _reportApiService;
    private readonly AuthApiService _authApiService;
    private readonly OrganizationApiService _organizationApiService;
    private readonly UserApiService _userApiService;

    public DashboardPage(
        AuthApiService authApiService,
        CategoryApiService categoryApiService,
        ItemApiService itemApiService,
        StockTransactionApiService stockTransactionApiService,
        ReportApiService reportApiService,
        OrganizationApiService organizationApiService,
        UserApiService userApiService)
    {
        InitializeComponent();
        _authApiService = authApiService;
        _categoryApiService = categoryApiService;
        _itemApiService = itemApiService;
        _stockTransactionApiService = stockTransactionApiService;
        _reportApiService = reportApiService;
        _organizationApiService = organizationApiService;
        _userApiService = userApiService;

        NavigationPage.SetHasBackButton(this, false);
    }

    private bool IsManager =>
        string.Equals(AppSession.Role, "Manager", StringComparison.OrdinalIgnoreCase);

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        WelcomeLabel.Text = $"Welcome, {AppSession.FullName}";
        OrganizationLabel.Text = AppSession.OrganizationName;
        OrganizationTypeLabel.Text = string.IsNullOrWhiteSpace(AppSession.OrganizationType)
            ? string.Empty
            : $"Type: {AppSession.OrganizationType}";
        EmailLabel.Text = $"Email: {AppSession.Email}";
        RoleLabel.Text = $"Role: {AppSession.Role}";
        OrganizationIdLabel.Text = $"Organization ID: {AppSession.OrganizationId}";

        StaffButton.IsVisible = IsManager;
        //DeletedUsersButton.IsVisible = IsManager;
        ExportExcelButton.IsVisible = IsManager;

        try
        {
            var categories = await _categoryApiService.GetByOrganizationAsync(AppSession.OrganizationId);
            var items = await _itemApiService.GetByOrganizationAsync(AppSession.OrganizationId);
            var shortageItems = await _itemApiService.GetShortageAsync(AppSession.OrganizationId);

            CategoriesCountLabel.Text = $"Categories: {categories.Count}";
            ItemsCountLabel.Text = $"Active Items: {items.Count(i => i.IsActive)}";
            ShortageCountLabel.Text = $"Low Stock Items: {shortageItems.Count}";
        }
        catch (Exception ex)
        {
            CategoriesCountLabel.Text = "Categories: error";
            ItemsCountLabel.Text = "Active Items: error";
            ShortageCountLabel.Text = $"Low Stock Items: error ({ex.Message})";
        }
    }

    private async void OnCategoriesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CategoriesPage(
            _categoryApiService,
            _itemApiService,
            _stockTransactionApiService));
    }

    private async void OnShortageItemsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ShortageItemsPage(
            _itemApiService,
            _categoryApiService,
            _stockTransactionApiService));
    }

    private async void OnStaffClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new StaffPage(_userApiService));
    }

    //private async void OnDeletedUsersClicked(object sender, EventArgs e)
    //{
    //    await Navigation.PushAsync(new DeletedUsersPage(_userApiService));
    //}

    private async void OnExportExcelReportClicked(object sender, EventArgs e)
    {
        if (!ExportExcelButton.IsEnabled)
            return;

        var originalText = "Export Excel Report";

        try
        {
            ExportExcelButton.IsEnabled = false;
            ExportExcelButton.Text = "Exporting...";

            var bytes = await _reportApiService.ExportExcelAsync(AppSession.OrganizationId);

            if (bytes == null || bytes.Length == 0)
            {
                ExportExcelButton.Text = originalText;
                await DisplayAlert("Error", "Failed to export Excel report.", "OK");
                return;
            }

            var safeOrganizationName = string.IsNullOrWhiteSpace(AppSession.OrganizationName)
                ? "Organization"
                : string.Concat(AppSession.OrganizationName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"InventoryReport_{safeOrganizationName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            File.WriteAllBytes(filePath, bytes);

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });

            await Task.Delay(2000);
            ExportExcelButton.Text = originalText;
        }
        catch (Exception ex)
        {
            ExportExcelButton.Text = originalText;
            await DisplayAlert("Error", $"Export failed: {ex.Message}", "OK");
        }
        finally
        {
            ExportExcelButton.IsEnabled = true;
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Logout",
            "Are you sure you want to logout?",
            "Yes",
            "No");

        if (!confirm)
            return;

        AppSession.FullName = string.Empty;
        AppSession.Email = string.Empty;
        AppSession.Role = string.Empty;
        AppSession.OrganizationName = string.Empty;
        AppSession.OrganizationId = Guid.Empty;
        AppSession.OrganizationType = string.Empty;

        Application.Current!.MainPage = new NavigationPage(new LoginPage(_authApiService,
                                             _categoryApiService,_itemApiService,
                                             _stockTransactionApiService,_reportApiService,
                                             _organizationApiService,_userApiService));
    }
}