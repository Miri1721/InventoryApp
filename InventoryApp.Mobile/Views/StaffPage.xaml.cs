using InventoryApp.Mobile.Models;
using InventoryApp.Mobile.Services;

namespace InventoryApp.Mobile.Views;

public partial class StaffPage : ContentPage
{
    private readonly UserApiService _userApiService;

    public StaffPage(UserApiService userApiService)
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

            var users = await _userApiService.GetActiveByOrganizationAsync(AppSession.OrganizationId);
            foreach (var user in users)
            {
                user.CanDelete = !string.Equals(user.Email, AppSession.Email, StringComparison.OrdinalIgnoreCase);
            }

            UsersCollectionView.ItemsSource = users;

            if (users.Count == 0)
                MessageLabel.Text = "No staff found.";
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to load staff: {ex.Message}";
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not UserModel user)
            return;

        bool confirm = await DisplayAlert(
            "Confirm",
            $"Are you sure you want to delete '{user.FullName}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        try
        {
            var success = await _userApiService.DeactivateAsync(user.UserId);

            if (!success)
            {
                MessageLabel.Text = "Failed to delete user.";
                return;
            }

            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Failed to delete user: {ex.Message}";
        }
    }

    private async void OnDeletedStaffUsersClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DeletedUsersPage(_userApiService));
    }
}