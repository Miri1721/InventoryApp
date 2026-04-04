namespace InventoryApp.Api.DTOs
{
    public class StockTransactionResponseDto
    {
        public Guid StockTransactionId { get; set; }
        public Guid ItemId { get; set; }
        public Guid OrganizationId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public double QuantityChange { get; set; }
        public double QuantityBefore { get; set; }
        public double QuantityAfter { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedByEmail { get; set; } = string.Empty;
    }
}