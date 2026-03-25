using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryApp.Api.Models
{
    [BsonIgnoreExtraElements]
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid CategoryId { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public Guid OrganizationId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}