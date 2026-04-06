using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class EditItemPage : ContentPage
{
    private readonly ItemApiService _itemApiService;
    private readonly ItemModel _item;

    public EditItemPage(ItemApiService itemApiService, ItemModel item)
    {
        InitializeComponent();
        _itemApiService = itemApiService;
        _item = item;

        NameEntry.Text = _item.Name;
        DescriptionEditor.Text = _item.Description;
        UnitEntry.Text = _item.Unit;
        MinimumThresholdEntry.Text = _item.MinimumThreshold.ToString();
        SupplierEntry.Text = _item.Supplier;
        SupplierPhoneEntry.Text = _item.SupplierPhone;
        SupplierEmailEntry.Text = _item.SupplierEmail;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        try
        {
            var name = NameEntry.Text?.Trim() ?? string.Empty;
            var description = DescriptionEditor.Text?.Trim() ?? string.Empty;
            var unit = UnitEntry.Text?.Trim() ?? string.Empty;
            var supplier = SupplierEntry.Text?.Trim() ?? string.Empty;
            var supplierPhone = SupplierPhoneEntry.Text?.Trim() ?? string.Empty;
            var supplierEmail = SupplierEmailEntry.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageLabel.Text = "Item name is required.";
                return;
            }

            if (!double.TryParse(MinimumThresholdEntry.Text, out var minimumThreshold))
            {
                MessageLabel.Text = "Minimum threshold must be a valid number.";
                return;
            }

            if (minimumThreshold < 0)
            {
                MessageLabel.Text = "Minimum threshold cannot be negative.";
                return;
            }

            var request = new UpdateItemRequest
            {
                Name = name,
                Description = description,
                Unit = unit,
                CurrentQuantity = _item.CurrentQuantity,
                MinimumThreshold = minimumThreshold,
                Supplier = supplier,
                SupplierPhone = supplierPhone,
                SupplierEmail = supplierEmail
            };

            var success = await _itemApiService.UpdateAsync(_item.ItemId, request);

            if (!success)
            {
                MessageLabel.Text = "Failed to update item.";
                return;
            }

            await DisplayAlert("Success", "Item updated successfully.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Error: {ex.Message}";
        }
    }
}