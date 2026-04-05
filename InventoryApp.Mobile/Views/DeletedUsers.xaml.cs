using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class DeletedUsersPage : ContentPage
{
    private readonly UserApiService _userApiService;

    public DeletedUsersPage(UserApiService userApiService)
    {
        InitializeComponent();
        _userApiService = userApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            MessageLabel.Text = string.Empty;

            var users = await _userApiService.GetDeletedByOrganizationAsync(AppSession.OrganizationId);
            UsersCollectionView.ItemsSource = users;

            if (users.Count == 0)
                MessageLabel.Text = "No deleted users found.";
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load deleted users: {ex.Message}";
        }
    }

    private async void OnRecoverClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not UserModel user)
            return;

        bool confirm = await DisplayAlert(
            "Confirm",
            $"Are you sure you want to recover '{user.FullName}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        try
        {
            var success = await _userApiService.ReactivateAsync(user.UserId);

            if (!success)
            {
                MessageLabel.Text = "Failed to recover user.";
                return;
            }

            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to recover user: {ex.Message}";
        }
    }
}