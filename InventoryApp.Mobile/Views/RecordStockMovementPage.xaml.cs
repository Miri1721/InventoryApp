using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class RecordStockMovementPage : ContentPage
{
    private readonly StockTransactionApiService _stockTransactionApiService;
    private readonly ItemModel _item;

    public RecordStockMovementPage(
        StockTransactionApiService stockTransactionApiService,
        ItemModel item)
    {
        InitializeComponent();
        _stockTransactionApiService = stockTransactionApiService;
        _item = item;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ItemNameLabel.Text = _item.Name;
        CurrentQuantityLabel.Text = $"Current Quantity: {_item.CurrentQuantity}";
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        try
        {
            var selectedType = TransactionTypePicker.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(selectedType))
            {
                MessageLabel.Text = "Please select a movement type.";
                return;
            }

            if (!double.TryParse(QuantityEntry.Text?.Trim(), out double quantity) || quantity <= 0)
            {
                MessageLabel.Text = "Please enter a valid quantity greater than 0.";
                return;
            }

            var request = new CreateStockTransactionRequest
            {
                ItemId = _item.ItemId,
                OrganizationId = AppSession.OrganizationId,
                TransactionType = selectedType,
                Quantity = quantity,
                Note = NoteEditor.Text?.Trim() ?? string.Empty,
                CreatedByEmail = AppSession.Email
            };

            var result = await _stockTransactionApiService.CreateAsync(request);

            if (result == null)
            {
                MessageLabel.Text = "Failed to save stock movement.";
                return;
            }

            await DisplayAlert("Success", "Stock movement recorded successfully.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Error: {ex.Message}";
        }
    }
}