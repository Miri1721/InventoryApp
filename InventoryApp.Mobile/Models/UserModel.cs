namespace InventoryApp.Mobile.Models
{
    public class UserModel
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
        public bool IsActive { get; set; }
        public bool CanDelete { get; set; }
        //public bool CanDelete => !string.Equals(Email, AppSession.Email, StringComparison.OrdinalIgnoreCase);
    }
}