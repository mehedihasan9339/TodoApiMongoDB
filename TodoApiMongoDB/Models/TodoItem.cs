using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TodoApiMongoDB.Models
{
    public class TodoItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
