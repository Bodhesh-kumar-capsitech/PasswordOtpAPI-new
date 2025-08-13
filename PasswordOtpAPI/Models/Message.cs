using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PasswordOtpAPI.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public List<Messagecontainer>? Messagecontainers { get; set; }
        
    }

    public class Messagecontainer
    {
        [BsonElement("User")]
        public string User { get; set; } = string.Empty;

        [BsonElement("Messages")]
        public string Messages { get; set; } = string.Empty;

        [BsonElement("SentAt")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;


    }

    public class Session
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public List<Message>? Messages { get; set; } = new();
    }
}
