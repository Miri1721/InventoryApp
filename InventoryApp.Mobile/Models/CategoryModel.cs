namespace InventoryApp.Mobile.Models
{
    public class CategoryModel
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
    }
}