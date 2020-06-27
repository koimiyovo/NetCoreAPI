using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext todoContext;

        public TodoItemsController(TodoContext context)
        {
            todoContext = context;
        }

        // Check if a todoItem exists
        private bool TodoItemExists(long id) => todoContext.TodoItems.Any(i => i.Id == id);

        // GET: api/todoitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            var todoItems = await todoContext.TodoItems.ToListAsync();

            return Ok(todoItems);
        }

        // GET: api/todoitems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var item = await todoContext.TodoItems.FindAsync(id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/todoitems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateTodoItem([FromBody] TodoItem todoItem)
        {
            todoContext.TodoItems.Add(todoItem);
            await todoContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // PUT: api/todoitems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, [FromBody] TodoItem todoItem)
        {
            if (id != todoItem.Id)
                return BadRequest();

            todoContext.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await todoContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/todoitems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await todoContext.TodoItems.FindAsync(id);

            if (todoItem == null)
                return NotFound();

            todoContext.TodoItems.Remove(todoItem);
            await todoContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
