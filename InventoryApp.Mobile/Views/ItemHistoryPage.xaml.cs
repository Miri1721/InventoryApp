using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class ItemHistoryPage : ContentPage
{
    private readonly StockTransactionApiService _stockTransactionApiService;
    private readonly ItemModel _item;

    public ItemHistoryPage(
        StockTransactionApiService stockTransactionApiService,
        ItemModel item)
    {
        InitializeComponent();
        _stockTransactionApiService = stockTransactionApiService;
        _item = item;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        ItemNameLabel.Text = $"{_item.Name} - History";

        try
        {
            MessageLabel.Text = string.Empty;

            var history = await _stockTransactionApiService.GetByItemAsync(_item.ItemId);

            HistoryCollectionView.ItemsSource = history;

            if (history.Count == 0)
            {
                MessageLabel.Text = "No stock movements found for this item.";
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load item history: {ex.Message}";
        }
    }
}