using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoryApiService _categoryApiService;
    private readonly ItemApiService _itemApiService;

    private List<CategoryModel> _allCategories = new();
    private readonly StockTransactionApiService _stockTransactionApiService;

    public CategoriesPage(
       CategoryApiService categoryApiService,
       ItemApiService itemApiService,
       StockTransactionApiService stockTransactionApiService)
    {
        InitializeComponent();
        _categoryApiService = categoryApiService;
        _itemApiService = itemApiService;
        _stockTransactionApiService = stockTransactionApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load categories: {ex.Message}";
        }
    }

    private async Task LoadCategoriesAsync()
    {
        MessageLabel.Text = string.Empty;

        var categories = await _categoryApiService.GetByOrganizationAsync(AppSession.OrganizationId);
        _allCategories = categories;

        bool hasCategories = _allCategories.Count > 0;
        SortPickerGrid.IsVisible = hasCategories;
        CategorySearchBar.IsVisible = hasCategories;

        if (hasCategories && SortPicker.SelectedIndex < 0)
            SortPicker.SelectedIndex = 0;

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        IEnumerable<CategoryModel> result = _allCategories;

        var searchText = CategorySearchBar.Text?.Trim();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            result = result.Where(c =>
                (!string.IsNullOrWhiteSpace(c.Name) &&
                 c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(c.Description) &&
                 c.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
        }

        switch (SortPicker.SelectedIndex)
        {
            case 1:
                result = result.OrderByDescending(c => c.Name);
                break;

            case 0:
            default:
                result = result.OrderBy(c => c.Name);
                break;
        }

        var filteredList = result.ToList();
        CategoriesCollectionView.ItemsSource = filteredList;

        MessageLabel.Text = filteredList.Count == 0
            ? "No categories found."
            : string.Empty;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void OnSortChanged(object sender, EventArgs e)
    {
        ApplyFilters();
    }

    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateCategoryPage(_categoryApiService));
    }

    private async void OnViewItemsClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is CategoryModel category)
        {
            await Navigation.PushAsync(
                new CategoryItemsPage(_itemApiService, _categoryApiService, _stockTransactionApiService, category));
        }
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

            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to deactivate category: {ex.Message}";
        }
    }

    private async void OnReactivateClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not CategoryModel category)
            return;

        bool confirm = await DisplayAlert(
            "Confirm",
            $"Are you sure you want to reactivate '{category.Name}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        try
        {
            var success = await _categoryApiService.ReactivateAsync(category.CategoryId);

            if (!success)
            {
                MessageLabel.Text = "Failed to reactivate category.";
                return;
            }

            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to reactivate category: {ex.Message}";
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not CategoryModel category)
            return;

        bool confirm = await DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to delete '{category.Name}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        try
        {
            var success = await _categoryApiService.DeleteAsync(category.CategoryId);

            if (!success)
            {
                MessageLabel.Text = "Failed to delete category.";
                return;
            }

            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to delete category: {ex.Message}";
        }
    }
}