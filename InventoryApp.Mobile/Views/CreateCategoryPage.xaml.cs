using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class CreateCategoryPage : ContentPage
{
    private readonly CategoryApiService _categoryApiService;

    public CreateCategoryPage(CategoryApiService categoryApiService)
    {
        InitializeComponent();
        _categoryApiService = categoryApiService;
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

            var request = new CreateCategoryRequest
            {
                Name = name,
                Description = description,
                OrganizationId = AppSession.OrganizationId
            };

            var success = await _categoryApiService.CreateAsync(request);

            if (!success)
            {
                MessageLabel.Text = "Failed to create category.";
                return;
            }

            await DisplayAlert("Success", "Category created successfully.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Error: {ex.Message}";
        }
    }
}