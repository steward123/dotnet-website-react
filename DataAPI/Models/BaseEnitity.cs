using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DataAPI.Models
{
    public class BaseEnitity :IBaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    }
}
