using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class EditCategoryPage : ContentPage
{
    private readonly CategoryApiService _categoryApiService;
    private readonly CategoryModel _category;

    public EditCategoryPage(CategoryApiService categoryApiService, CategoryModel category)
    {
        InitializeComponent();
        _categoryApiService = categoryApiService;
        _category = category;

        NameEntry.Text = _category.Name;
        DescriptionEditor.Text = _category.Description;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        try
        {
            var name = NameEntry.Text?.Trim() ?? string.Empty;
            var description = DescriptionEditor.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageLabel.Text = "Category name is required.";
                return;
            }

            var request = new UpdateCategoryRequest
            {
                Name = name,
                Description = description
            };

            var success = await _categoryApiService.UpdateAsync(_category.CategoryId, request);

            if (!success)
            {
                MessageLabel.Text = "Failed to update category.";
                return;
            }

            await DisplayAlert("Success", "Category updated successfully.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Error: {ex.Message}";
        }
    }
}