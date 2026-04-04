namespace InventoryApp.Mobile.Models
{
    public class OrganizationModel
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public string DisplayName => $"{Name} ({Type})";
    }
}