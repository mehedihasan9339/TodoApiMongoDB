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
            return await _context.TodoItems.Find(todo => true).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> Get(string id)
        {
            var todoItem = await _context.TodoItems.Find(todo => todo.Id == id).FirstOrDefaultAsync();

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
            var result = await _context.TodoItems.ReplaceOneAsync(todo => todo.Id == id, updatedTodoItem);

            if (result.MatchedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _context.TodoItems.DeleteOneAsync(todo => todo.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
