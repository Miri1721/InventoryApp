using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class CategoryItemsPage : ContentPage
{
    private readonly ItemApiService _itemApiService;
    private readonly CategoryApiService _categoryApiService;
    private readonly CategoryModel _category;

    public CategoryItemsPage(ItemApiService itemApiService, CategoryApiService categoryApiService, CategoryModel category)
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
        CategoryDescriptionLabel.Text = _category.Description;

        try
        {
            MessageLabel.Text = string.Empty;

            var allItems = await _itemApiService.GetByOrganizationAsync(AppSession.OrganizationId);
            var categoryItems = allItems
                .Where(i => i.CategoryId == _category.CategoryId)
                .ToList();

            ItemsCollectionView.ItemsSource = categoryItems;

            if (categoryItems.Count == 0)
            {
                MessageLabel.Text = "No items found in this category.";
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load items: {ex.Message}";
        }
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
            var categoryItems = allItems
                .Where(i => i.CategoryId == _category.CategoryId)
                .ToList();

            ItemsCollectionView.ItemsSource = categoryItems;
            MessageLabel.Text = categoryItems.Count == 0 ? "No items found in this category." : string.Empty;
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to deactivate item: {ex.Message}";
        }
    }
}