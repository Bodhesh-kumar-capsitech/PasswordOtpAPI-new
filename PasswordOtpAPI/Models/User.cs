using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PasswordOtpAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("contactNumber")]
        public string ContactNumber { get; set; } = string.Empty;

        [BsonElement("otpCode")]
        public string OtpCode { get; set; } = string.Empty;

        [BsonElement("otpExpiryTime")]
        public DateTime? OtpExpiryTime { get; set; } = DateTime.UtcNow.AddMinutes(10);
    }
}

public class Apiresponse<T>
{
    public string Message { get; set; } = string.Empty;

    public bool Status { get; set; } = false;

    public T? Result { get; set; }


}
