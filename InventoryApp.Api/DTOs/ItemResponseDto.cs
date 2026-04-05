namespace InventoryApp.Api.DTOs
{
    public class ItemResponseDto
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double CurrentQuantity { get; set; }
        public double MinimumThreshold { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public bool IsBelowThreshold { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}