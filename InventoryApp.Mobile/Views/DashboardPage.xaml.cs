using InventoryApp.Mobile.Models;

namespace InventoryApp.Mobile.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(AuthResponse response)
    {
        InitializeComponent();

        WelcomeLabel.Text = $"Welcome, {response.FullName}";
        EmailLabel.Text = $"Email: {response.Email}";
        RoleLabel.Text = $"Role: {response.Role}";
        OrganizationLabel.Text = $"Organization: {response.OrganizationName}";
    }
}