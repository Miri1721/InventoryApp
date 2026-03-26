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

            var items = await _itemApiService.GetShortageAsync(AppSession.OrganizationId);
            ShortageItemsCollectionView.ItemsSource = items;

            if (items.Count == 0)
            {
                MessageLabel.Text = "No shortage items found.";
            }
            else
            {
                MessageLabel.Text = string.Empty;
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to deactivate item: {ex.Message}";
        }
    }
}