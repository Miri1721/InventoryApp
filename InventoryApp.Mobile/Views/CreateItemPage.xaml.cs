using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class CreateItemPage : ContentPage
{
    private readonly ItemApiService _itemApiService;
    private readonly CategoryApiService _categoryApiService;
    private readonly CategoryModel? _preselectedCategory;

    public CreateItemPage(ItemApiService itemApiService, CategoryApiService categoryApiService, CategoryModel? preselectedCategory = null)
    {
        InitializeComponent();
        _itemApiService = itemApiService;
        _categoryApiService = categoryApiService;
        _preselectedCategory = preselectedCategory;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            MessageLabel.Text = string.Empty;

            if (_preselectedCategory != null)
            {
                CategoryNameLabel.Text = _preselectedCategory.Name;
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load categories: {ex.Message}";
        }
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

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageLabel.Text = "Item name is required.";
                return;
            }

            if (!double.TryParse(CurrentQuantityEntry.Text, out var currentQuantity))
            {
                MessageLabel.Text = "Current quantity must be a valid number.";
                return;
            }

            if (!double.TryParse(MinimumThresholdEntry.Text, out var minimumThreshold))
            {
                MessageLabel.Text = "Minimum threshold must be a valid number.";
                return;
            }

            if (_preselectedCategory == null)
            {
                MessageLabel.Text = "Category is missing.";
                return;
            }

            var request = new CreateItemRequest
            {
                Name = name,
                Description = description,
                CategoryId = _preselectedCategory.CategoryId,
                OrganizationId = AppSession.OrganizationId,
                Unit = unit,
                CurrentQuantity = currentQuantity,
                MinimumThreshold = minimumThreshold,
                Supplier = supplier,
            };

            var success = await _itemApiService.CreateAsync(request);

            if (!success)
            {
                MessageLabel.Text = "Failed to create item.";
                return;
            }

            await DisplayAlert("Success", "Item created successfully.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Error: {ex.Message}";
        }
    }
}