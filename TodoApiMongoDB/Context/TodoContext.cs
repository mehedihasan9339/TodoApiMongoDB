using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoApiMongoDB.Models;

namespace TodoApiMongoDB.Context
{
    public class TodoContext
    {
        private readonly IMongoDatabase _database;

        public TodoContext(IOptions<TodoDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<TodoItem> TodoItems => _database.GetCollection<TodoItem>("TodoItems");
    }

    public class TodoDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
