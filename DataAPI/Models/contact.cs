using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace DataAPI.Models
{
    public class contact :BaseEnitity
    {

        //[BsonElement("Name")]
        public string Name { get; set; }
        //[BsonElement("Email")]
        public string Email { get; set; }
        //[BsonElement("Message")]
        public string message { get; set; }
    }
}
