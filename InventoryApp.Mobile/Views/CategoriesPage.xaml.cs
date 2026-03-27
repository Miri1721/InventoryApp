using InventoryApp.Mobile.Services;
using InventoryApp.Mobile.Models;

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


    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is CategoryModel category)
        {
            await Navigation.PushAsync(new EditCategoryPage(_categoryApiService, category));
        }
    }

    private async void OnDeactivateClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not CategoryModel category)
            return;

        bool confirm = await DisplayAlert(
            "Confirm",
            $"Are you sure you want to deactivate '{category.Name}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        try
        {
            var success = await _categoryApiService.DeactivateAsync(category.CategoryId);

            if (!success)
            {
                MessageLabel.Text = "Failed to deactivate category.";
                return;
            }

            var categories = await _categoryApiService.GetByOrganizationAsync(AppSession.OrganizationId);
            CategoriesCollectionView.ItemsSource = categories;

            MessageLabel.Text = categories.Count == 0 ? "No categories found." : string.Empty;
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to deactivate category: {ex.Message}";
        }
    }
}