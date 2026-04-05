namespace InventoryApp.Api.DTOs
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
        public bool IsActive { get; set; }
    }
}