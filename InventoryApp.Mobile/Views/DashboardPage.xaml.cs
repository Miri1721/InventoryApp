using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class DashboardPage : ContentPage
{
    private readonly CategoryApiService _categoryApiService;
    private readonly ItemApiService _itemApiService;

    public DashboardPage(
        CategoryApiService categoryApiService,
        ItemApiService itemApiService)
    {
        InitializeComponent();
        _categoryApiService = categoryApiService;
        _itemApiService = itemApiService;
    }

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

        try
        {
            var categories = await _categoryApiService.GetByOrganizationAsync(AppSession.OrganizationId);
            var items = await _itemApiService.GetByOrganizationAsync(AppSession.OrganizationId);
            var shortageItems = await _itemApiService.GetShortageAsync(AppSession.OrganizationId);

            CategoriesCountLabel.Text = $"Categories: {categories.Count}";
            ItemsCountLabel.Text = $"Active Items: {items.Count}";
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
        await Navigation.PushAsync(new CategoriesPage(_categoryApiService, _itemApiService));
    }

    private async void OnShortageItemsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ShortageItemsPage(_itemApiService, _categoryApiService));
    }
}