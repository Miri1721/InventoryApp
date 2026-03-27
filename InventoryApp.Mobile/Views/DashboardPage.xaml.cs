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
        OrganizationIdLabel.Text = $"Organization ID: {AppSession.OrganizationId}";
        EmailLabel.Text = $"Email: {AppSession.Email}";
        RoleLabel.Text = $"Role: {AppSession.Role}";
        OrganizationLabel.Text = $"Organization: {AppSession.OrganizationName}";

        try
        {
            var categories = await _categoryApiService.GetByOrganizationAsync(AppSession.OrganizationId);
            var items = await _itemApiService.GetByOrganizationAsync(AppSession.OrganizationId);
            var shortageItems = await _itemApiService.GetShortageAsync(AppSession.OrganizationId);

            CategoriesCountLabel.Text = $"Categories: {categories.Count}";
            ItemsCountLabel.Text = $"Active Items: {items.Count}";
            ShortageCountLabel.Text = $"Shortage Items: {shortageItems.Count}";
        }
        catch (Exception ex)
        {
            CategoriesCountLabel.Text = "Categories: error";
            ItemsCountLabel.Text = "Active Items: error";
            ShortageCountLabel.Text = $"Shortage Items: error ({ex.Message})";
        }
    }

    private async void OnCategoriesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CategoriesPage(_categoryApiService));
    }

    private async void OnItemsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ItemsPage(_itemApiService, _categoryApiService));
    }

    private async void OnShortageItemsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ShortageItemsPage(_itemApiService));
    }
}