using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TodoApiMongoDB.Models
{
    public class TodoItem : BaseEntity
    {
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
