namespace InventoryApp.Mobile.Services
{
    public static class AppSession
    {
        public static string FullName { get; set; } = string.Empty;
        public static string Email { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;
        public static string OrganizationName { get; set; } = string.Empty;

        public static Guid OrganizationId { get; set; }
    }
}