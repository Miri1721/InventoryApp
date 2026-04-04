using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryApp.Api.Models
{
    [BsonIgnoreExtraElements]
    public class StockTransaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid StockTransactionId { get; set; } = Guid.NewGuid();

        [BsonRepresentation(BsonType.String)]
        public Guid ItemId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OrganizationId { get; set; }

        public string TransactionType { get; set; } = string.Empty;
        // Restock / Usage / Waste / Correction

        public double QuantityChange { get; set; }

        public double QuantityBefore { get; set; }

        public double QuantityAfter { get; set; }

        public string Note { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string CreatedByEmail { get; set; } = string.Empty;
    }
}