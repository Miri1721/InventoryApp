namespace InventoryApp.Api.Models
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
        public bool IsActive { get; set; } = true;

        public string FullName => $"{FirstName} {LastName}";
    }
}