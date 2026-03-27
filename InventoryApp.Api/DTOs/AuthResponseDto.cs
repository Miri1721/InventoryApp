namespace InventoryApp.Api.DTOs
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? OrganizationName { get; set; }
        public Guid? OrganizationId { get; set; }
        public string? OrganizationType { get; set; }
    }
}