namespace InventoryApp.Api.DTOs
{
    public class CreateCategoryRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
    }
}