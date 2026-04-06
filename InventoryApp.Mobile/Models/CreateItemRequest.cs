namespace InventoryApp.Mobile.Models
{
    public class CreateItemRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double CurrentQuantity { get; set; }
        public double MinimumThreshold { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public string SupplierPhone { get; set; } = string.Empty;
        public string SupplierEmail { get; set; } = string.Empty;
    }
}