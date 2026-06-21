using CsharpTodo.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CsharpTodo.Api.Application.Todos;

public sealed class GetTodoQueryHandler(TodoDbContext database)
{
    public async Task<TodoDto?> HandleAsync(int id, CancellationToken cancellationToken)
    {
        return await database.Todos
            .AsNoTracking()
            .Where(todo => todo.Id == id)
            .Select(todo => new TodoDto(
                todo.Id,
                todo.Title,
                todo.Description,
                todo.IsCompleted,
                todo.CreatedAt,
                todo.Label == null ? null : new LabelDto(
                    todo.Label.Id,
                    todo.Label.Name,
                    todo.Label.Description,
                    todo.Label.Color)))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
