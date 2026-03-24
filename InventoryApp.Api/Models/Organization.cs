namespace InventoryApp.Api.Models
{
    public class Organization
    {
        public Guid OrganizationId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}