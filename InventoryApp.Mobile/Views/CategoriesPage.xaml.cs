using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoryApiService _categoryApiService;

    public CategoriesPage(CategoryApiService categoryApiService)
    {
        InitializeComponent();
        _categoryApiService = categoryApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            MessageLabel.Text = string.Empty;

            var categories = await _categoryApiService.GetByOrganizationAsync(AppSession.OrganizationId);
            CategoriesCollectionView.ItemsSource = categories;

            if (categories.Count == 0)
            {
                MessageLabel.Text = "No categories found.";
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load categories: {ex.Message}";
        }
    }

    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateCategoryPage(_categoryApiService));
    }
}