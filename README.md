# Integrating MongoDB with ASP.NET Core C# Web API

# Key Points for Using MongoDB

1. **Schemaless Design:** MongoDB is schemaless, allowing flexibility in document structure without predefined schemas. This flexibility is beneficial for rapidly changing data requirements.

2. **JSON-like Documents:** Data is stored in BSON (Binary JSON) format, which supports rich data types and is compatible with JSON. This makes data storage and manipulation straightforward.

3. **Scalability:** MongoDB is designed for horizontal scalability through sharding, distributing data across multiple servers. This allows handling large volumes of data and high throughput applications.

4. **Indexing:** Indexes can be created to improve query performance. MongoDB supports various types of indexes, including single field, compound, multikey, geospatial, and text indexes.

5. **Ad Hoc Queries:** MongoDB supports complex queries, aggregations, and filtering operations using its powerful query language. Queries can utilize indexing for efficient data retrieval.

6. **High Availability:** MongoDB provides replication for data redundancy and fault tolerance. Replica sets ensure automatic failover and data availability even in case of server failures.

7. **Flexible Aggregation Framework:** MongoDB's aggregation framework allows performing aggregation operations, such as grouping, sorting, and transforming data, directly within the database.

8. **No Migration Required: MongoDB's schemaless nature means that there's no need for migrations when evolving the schema or adding new fields. Applications can seamlessly handle new data structures without downtime or complex migration scripts.**

9. **Transactions:** MongoDB supports multi-document ACID transactions in replica sets starting from version 4.0, ensuring data consistency and integrity across multiple operations.

10. **Document Updates:** MongoDB supports atomic operations on single documents, allowing safe updates without affecting other concurrent operations.

11. **Community Support and Tools:** MongoDB has a large community, extensive documentation, and a range of tools (e.g., Compass, MongoDB Atlas) for monitoring, backup, and management.

12. **Use Cases:** MongoDB is suitable for various use cases including content management, real-time analytics, IoT data storage, mobile applications, and e-commerce platforms.

## Considerations

- **Data Modeling:** Designing optimal schemas based on application requirements is crucial for performance and scalability.
  
- **Indexing Strategy:** Proper indexing improves query performance but requires understanding query patterns and access patterns.

- **Deployment:** Choose between self-managed deployments, MongoDB Atlas (cloud-hosted), or other managed services based on scalability, management, and cost requirements.





## Steps to Integrate MongoDB

1. **Install MongoDB Driver for C#:** Add the MongoDB driver to your project using NuGet Package Manager or .NET CLI.
   
```csharp
dotnet add package MongoDB.Driver
```
2. **Configure MongoDB Connection:** Update your appsettings.json or appsettings.Development.json with your MongoDB connection string.

```csharp
{
  "ConnectionStrings": {
    "MongoDBConnection": "mongodb://localhost:27017/YourDatabaseName"
  }
}
```

3. **Configure ConnectionString in Program:** Add this in Program.cs file to cofigure ConnectionString
```csharp
## Adding Configuration and Services in Program.cs

In an ASP.NET Core application, configuration and service registration are typically set up in the `Program.cs` file. Here's an example of how `TodoDatabaseSettings` and `TodoContext` are configured and registered:

```csharp
// Program.cs

builder.Services.Configure<TodoDatabaseSettings>(
    builder.Configuration.GetSection(nameof(TodoDatabaseSettings)));

builder.Services.AddSingleton<TodoContext>();
```


4. **Create MongoDB Context:** Implement a MongoDB context class to manage database connection and collections.
```csharp
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
```

5. **Define MongoDB Model:** Create a model class to represent MongoDB documents.
```csharp
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




namespace TodoApiMongoDB.Models
{
    public class TodoItem : BaseEntity
    {
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}

```

7. **Implement MongoDB Controllers:** Inject Context in your ASP.NET Core Web API controllers to perform CRUD operations.
```csharp
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TodoApiMongoDB.Context;
using TodoApiMongoDB.Models;

namespace TodoApiMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> Get()
        {
            return await _context.TodoItems.Find(x => true).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> Get(string id)
        {
            var todoItem = await _context.TodoItems.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> Create(TodoItem todoItem)
        {
            await _context.TodoItems.InsertOneAsync(todoItem);

            return CreatedAtAction(nameof(Get), new { id = todoItem.Id }, todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, TodoItem updatedTodoItem)
        {
            var result = await _context.TodoItems.ReplaceOneAsync(x => x.Id == id, updatedTodoItem);

            if (result.MatchedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _context.TodoItems.DeleteOneAsync(x => x.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
```

