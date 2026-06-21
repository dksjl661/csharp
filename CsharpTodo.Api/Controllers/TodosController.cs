using CsharpTodo.Api.Application.Todos;
using Microsoft.AspNetCore.Mvc;

namespace CsharpTodo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController(
    GetTodosQueryHandler getTodos,
    GetTodoQueryHandler getTodo,
    CreateTodoCommandHandler createTodo,
    UpdateTodoCommandHandler updateTodo,
    DeleteTodoCommandHandler deleteTodo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoDto>>> GetTodos(CancellationToken cancellationToken)
    {
        return Ok(await getTodos.HandleAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoDto>> GetTodo(int id, CancellationToken cancellationToken)
    {
        var todo = await getTodo.HandleAsync(id, cancellationToken);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<TodoDto>> CreateTodo(TodoInput input, CancellationToken cancellationToken)
    {
        if (!input.IsValid(out var error))
        {
            return BadRequest(new { error });
        }

        var result = await createTodo.HandleAsync(input, cancellationToken);
        return result.Todo is null
            ? BadRequest(new { error = result.Error })
            : CreatedAtAction(nameof(GetTodo), new { id = result.Todo.Id }, result.Todo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoDto>> UpdateTodo(int id, TodoInput input, CancellationToken cancellationToken)
    {
        if (!input.IsValid(out var error))
        {
            return BadRequest(new { error });
        }

        var result = await updateTodo.HandleAsync(id, input, cancellationToken);
        if (result.NotFound)
        {
            return NotFound();
        }

        return result.Todo is null ? BadRequest(new { error = result.Error }) : Ok(result.Todo);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTodo(int id, CancellationToken cancellationToken)
    {
        return await deleteTodo.HandleAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
