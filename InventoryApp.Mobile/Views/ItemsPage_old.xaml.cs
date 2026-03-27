using InventoryApp.Mobile.Services;
using InventoryApp.Mobile.Models;

namespace InventoryApp.Mobile.Views
{
    public partial class ItemsPage : ContentPage
    {
        private readonly ItemApiService _itemApiService;
        private readonly CategoryApiService _categoryApiService;

        public ItemsPage(ItemApiService itemApiService, CategoryApiService categoryApiService)
        {
            InitializeComponent();
            _itemApiService = itemApiService;
            _categoryApiService = categoryApiService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                MessageLabel.Text = string.Empty;

                var items = await _itemApiService.GetByOrganizationAsync(AppSession.OrganizationId);
                ItemsCollectionView.ItemsSource = items;

                if (items.Count == 0)
                {
                    MessageLabel.Text = "No items found.";
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = $"Failed to load items: {ex.Message}";
            }
        }

        private async void OnAddItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateItemPage(_itemApiService, _categoryApiService));
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

                var items = await _itemApiService.GetByOrganizationAsync(AppSession.OrganizationId);
                ItemsCollectionView.ItemsSource = items;

                if (items.Count == 0)
                {
                    MessageLabel.Text = "No items found.";
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
}