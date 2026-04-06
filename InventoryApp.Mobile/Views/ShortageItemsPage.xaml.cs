using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class ShortageItemsPage : ContentPage
{
    private readonly ItemApiService _itemApiService;
    private readonly CategoryApiService _categoryApiService;

    private List<ItemModel> _allShortageItems = new();
    private readonly StockTransactionApiService _stockTransactionApiService;
    private readonly ReportApiService _reportApiService;

    public ShortageItemsPage(ItemApiService itemApiService,
                          CategoryApiService categoryApiService,
                          StockTransactionApiService stockTransactionApiService,
                          ReportApiService reportApiService)
    {
        InitializeComponent();
        _itemApiService = itemApiService;
        _categoryApiService = categoryApiService;
        _stockTransactionApiService = stockTransactionApiService;
        _reportApiService = reportApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadShortageItemsAsync();
    }

    private async Task LoadShortageItemsAsync()
    {
        try
        {
            MessageLabel.Text = string.Empty;

            var items = await _itemApiService.GetShortageAsync(AppSession.OrganizationId);
            var categories = await _categoryApiService.GetByOrganizationAsync(AppSession.OrganizationId);

            var categoryMap = categories.ToDictionary(c => c.CategoryId, c => c.Name);

            foreach (var item in items)
            {
                if (categoryMap.TryGetValue(item.CategoryId, out var categoryName))
                    item.CategoryName = categoryName;
            }

            _allShortageItems = items;

            CategorySearchBar.Text = string.Empty;
            ItemSearchBar.Text = string.Empty;

            SearchSection.IsVisible = _allShortageItems.Count > 0;

            ApplyFilters();

            if (_allShortageItems.Count == 0)
            {
                MessageLabel.Text = "No shortage items found.";
            }
        }
        catch (Exception ex)
        {
            SearchSection.IsVisible = false;
            ShortageItemsCollectionView.ItemsSource = null;
            MessageLabel.Text = $"Failed to load shortage items: {ex.Message}";
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        if (_allShortageItems == null || _allShortageItems.Count == 0)
        {
            ShortageItemsCollectionView.ItemsSource = null;
            SearchSection.IsVisible = false;
            return;
        }

        var categorySearchText = CategorySearchBar.Text?.Trim() ?? string.Empty;
        var itemSearchText = ItemSearchBar.Text?.Trim() ?? string.Empty;

        var filteredItems = _allShortageItems
            .Where(item =>
                (string.IsNullOrWhiteSpace(categorySearchText) ||
                 (!string.IsNullOrWhiteSpace(item.CategoryName) &&
                  item.CategoryName.Contains(categorySearchText, StringComparison.OrdinalIgnoreCase)))
                &&
                (string.IsNullOrWhiteSpace(itemSearchText) ||
                 (!string.IsNullOrWhiteSpace(item.Name) &&
                  item.Name.Contains(itemSearchText, StringComparison.OrdinalIgnoreCase))))
            .ToList();

        ShortageItemsCollectionView.ItemsSource = filteredItems;

        MessageLabel.Text = filteredItems.Count == 0
            ? "No shortage items match your search."
            : string.Empty;
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

            await LoadShortageItemsAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to deactivate item: {ex.Message}";
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not ItemModel item)
            return;

        bool confirm = await DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to delete '{item.Name}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        try
        {
            var success = await _itemApiService.DeleteAsync(item.ItemId);

            if (!success)
            {
                MessageLabel.Text = "Failed to delete item.";
                return;
            }

            await LoadShortageItemsAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to delete item: {ex.Message}";
        }
    }

    private async void OnRecordMovementClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ItemModel item)
        {
            await Navigation.PushAsync(new RecordStockMovementPage(_stockTransactionApiService, item));
        }
    }

    private async void OnViewHistoryClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ItemModel item)
        {
            await Navigation.PushAsync(new ItemHistoryPage(_stockTransactionApiService, item));
        }
    }

    private async void OnExportReorderReportClicked(object sender, EventArgs e)
    {
        if (!ExportReorderReportButton.IsEnabled)
            return;

        var originalText = "Export Reorder Report";

        try
        {
            ExportReorderReportButton.IsEnabled = false;
            ExportReorderReportButton.Opacity = 0.7;
            ExportReorderReportButton.Text = "Exporting...";

            var bytes = await _reportApiService.ExportShortageExcelAsync(AppSession.OrganizationId);

            if (bytes == null || bytes.Length == 0)
            {
                ExportReorderReportButton.Text = originalText;
                MessageLabel.Text = "Failed to export reorder report.";
                return;
            }

            var safeOrganizationName = string.IsNullOrWhiteSpace(AppSession.OrganizationName)
                ? "Organization"
                : string.Concat(AppSession.OrganizationName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"ReorderReport_{safeOrganizationName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            File.WriteAllBytes(filePath, bytes);

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });

            await Task.Delay(2000);
            ExportReorderReportButton.Text = originalText;
        }
        catch (Exception ex)
        {
            ExportReorderReportButton.Text = originalText;
            MessageLabel.Text = $"Failed to export reorder report: {ex.Message}";
        }
        finally
        {
            ExportReorderReportButton.IsEnabled = true;
            ExportReorderReportButton.Opacity = 1;
        }
    }
}