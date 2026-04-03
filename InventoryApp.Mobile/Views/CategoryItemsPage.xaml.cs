using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class CategoryItemsPage : ContentPage
{
    private readonly ItemApiService _itemApiService;
    private readonly CategoryApiService _categoryApiService;
    private readonly CategoryModel _category;

    private List<ItemModel> _allItems = new();

    public CategoryItemsPage(
        ItemApiService itemApiService,
        CategoryApiService categoryApiService,
        CategoryModel category)
    {
        InitializeComponent();
        _itemApiService = itemApiService;
        _categoryApiService = categoryApiService;
        _category = category;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        CategoryTitleLabel.Text = _category.Name;
        CategoryDescriptionLabel.Text = _category.Description ?? string.Empty;

        try
        {
            MessageLabel.Text = string.Empty;

            var allItems = await _itemApiService.GetByOrganizationAsync(AppSession.OrganizationId);

            _allItems = allItems
                .Where(i => i.CategoryId == _category.CategoryId)
                .ToList();

            if (SortPicker.SelectedIndex < 0)
                SortPicker.SelectedIndex = 0;

            ApplyFilters();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load items: {ex.Message}";
        }
    }

    private void ApplyFilters()
    {
        IEnumerable<ItemModel> result = _allItems;

        var searchText = ItemSearchBar.Text?.Trim();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            result = result.Where(i =>
                (!string.IsNullOrWhiteSpace(i.Name) &&
                 i.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(i.Description) &&
                 i.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(i.Unit) &&
                 i.Unit.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
        }

        switch (SortPicker.SelectedIndex)
        {
            case 1:
                result = result.OrderByDescending(i => i.Name);
                break;

            case 2:
                result = result.OrderBy(i => i.CurrentQuantity);
                break;

            case 3:
                result = result.OrderByDescending(i => i.CurrentQuantity);
                break;

            case 0:
            default:
                result = result.OrderBy(i => i.Name);
                break;
        }

        var filteredList = result.ToList();
        ItemsCollectionView.ItemsSource = filteredList;

        MessageLabel.Text = filteredList.Count == 0
            ? "No items found in this category."
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

    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateItemPage(_itemApiService, _categoryApiService, _category));
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ItemModel item)
        {
            await Navigation.PushAsync(new EditItemPage(_itemApiService, item));
        }
    }

    private async void OnDeactivateClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not ItemModel item)
            return;

        bool confirm = await DisplayAlert(
            "Confirm",
            $"Are you sure you want to deactivate '{item.Name}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        try
        {
            var success = await _itemApiService.DeactivateAsync(item.ItemId);

            if (!success)
            {
                MessageLabel.Text = "Failed to deactivate item.";
                return;
            }

            var allItems = await _itemApiService.GetByOrganizationAsync(AppSession.OrganizationId);

            _allItems = allItems
                .Where(i => i.CategoryId == _category.CategoryId)
                .ToList();

            ApplyFilters();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to deactivate item: {ex.Message}";
        }
    }
}