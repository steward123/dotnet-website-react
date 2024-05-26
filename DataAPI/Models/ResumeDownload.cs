using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DataAPI.Models
{
    public class ResumeDownload :BaseEnitity
    {
        public DateTime? LastModified { get; set; }

        public string? guid_generated { get; set; }

        public contact contact { get; set; }
    }
}
