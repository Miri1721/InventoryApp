using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class ShortageItemsPage : ContentPage
{
    private readonly ItemApiService _itemApiService;

    public ShortageItemsPage(ItemApiService itemApiService)
    {
        InitializeComponent();
        _itemApiService = itemApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            MessageLabel.Text = string.Empty;

            var items = await _itemApiService.GetShortageAsync(AppSession.OrganizationId);
            ShortageItemsCollectionView.ItemsSource = items;

            if (items.Count == 0)
            {
                MessageLabel.Text = "No shortage items found.";
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load shortage items: {ex.Message}";
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ItemModel item)
        {
            await Navigation.PushAsync(new EditItemPage(_itemApiService, item));
        }
    }
}