using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryApp.Api.Models
{
    public class Organization
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid OrganizationId { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}