namespace InventoryApp.Api.DTOs
{
    public class CreateStockTransactionRequestDto
    {
        public Guid ItemId { get; set; }
        public Guid OrganizationId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string Note { get; set; } = string.Empty;
        public string CreatedByEmail { get; set; } = string.Empty;
    }
}