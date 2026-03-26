namespace InventoryApp.Mobile.Models
{
    public class ItemModel
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double CurrentQuantity { get; set; }
        public double MinimumThreshold { get; set; }
        public bool IsBelowThreshold { get; set; }
    }
}