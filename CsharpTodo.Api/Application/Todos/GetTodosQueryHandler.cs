using CsharpTodo.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CsharpTodo.Api.Application.Todos;

public sealed class GetTodosQueryHandler(TodoDbContext database)
{
    public async Task<IReadOnlyList<TodoDto>> HandleAsync(CancellationToken cancellationToken)
    {
        return await database.Todos
            .AsNoTracking()
            .Include(todo => todo.Label)
            .OrderBy(todo => todo.Id)
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
            .ToListAsync(cancellationToken);
    }
}
