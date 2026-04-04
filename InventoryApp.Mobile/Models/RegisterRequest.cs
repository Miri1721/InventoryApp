namespace InventoryApp.Mobile.Models
{
    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool IsExistingOrganization { get; set; }
        public Guid? ExistingOrganizationId { get; set; }

        public string OrganizationName { get; set; } = string.Empty;
        public string OrganizationType { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}