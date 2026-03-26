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

    protected override void OnAppearing()
    {
        base.OnAppearing();

        WelcomeLabel.Text = $"Welcome, {AppSession.FullName}";
        EmailLabel.Text = $"Email: {AppSession.Email}";
        RoleLabel.Text = $"Role: {AppSession.Role}";
        OrganizationLabel.Text = $"Organization: {AppSession.OrganizationName}";
        OrganizationIdLabel.Text = $"Organization ID: {AppSession.OrganizationId}";
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