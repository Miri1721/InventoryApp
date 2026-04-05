using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryApp.Api.Models
{
    [BsonIgnoreExtraElements]
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid ItemId { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public Guid CategoryId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OrganizationId { get; set; }

        public string Unit { get; set; } = string.Empty;
        public double CurrentQuantity { get; set; }
        public double MinimumThreshold { get; set; }
        public string Supplier { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}