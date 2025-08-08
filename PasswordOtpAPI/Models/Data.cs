using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PasswordOtpAPI.Models
{
    public class Data
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Task")]
        public string Taskname { get; set; } = string.Empty;

        [BsonElement("Description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("Status")]
        public bool Status { get; set; }

        [BsonElement("userId")]
        public string? CreatedBy { get; set; }
    }
}