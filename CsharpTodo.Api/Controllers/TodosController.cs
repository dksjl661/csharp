using CsharpTodo.Api.Contracts;
using CsharpTodo.Api.Data;
using CsharpTodo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CsharpTodo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController(TodoDbContext database) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TaskItem>>> GetTodos()
    {
        return Ok(await database.Todos.OrderBy(todo => todo.Id).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetTodo(int id)
    {
        var todo = await database.Todos.FindAsync(id);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTodo(TodoRequest request)
    {
        if (!request.IsValid(out var error))
        {
            return BadRequest(new { error });
        }

        var todo = new TaskItem
        {
            Title = request.Title!.Trim(),
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            CreatedAt = DateTime.UtcNow
        };

        database.Todos.Add(todo);
        await database.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskItem>> UpdateTodo(int id, TodoRequest request)
    {
        if (!request.IsValid(out var error))
        {
            return BadRequest(new { error });
        }

        var todo = await database.Todos.FindAsync(id);
        if (todo is null)
        {
            return NotFound();
        }

        todo.Title = request.Title!.Trim();
        todo.Description = request.Description;
        todo.IsCompleted = request.IsCompleted;
        await database.SaveChangesAsync();

        return Ok(todo);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await database.Todos.FindAsync(id);
        if (todo is null)
        {
            return NotFound();
        }

        database.Todos.Remove(todo);
        await database.SaveChangesAsync();

        return NoContent();
    }
}
