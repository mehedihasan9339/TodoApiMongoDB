using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TodoApiMongoDB.Models
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null;

        public BaseEntity()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = ObjectId.GenerateNewId().ToString();
            }
        }
    }
}
