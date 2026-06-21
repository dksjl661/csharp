using CsharpTodo.Api.Data;

namespace CsharpTodo.Api.Application.Todos;

public sealed class DeleteTodoCommandHandler(TodoDbContext database)
{
    public async Task<bool> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var todo = await database.Todos.FindAsync([id], cancellationToken);
        if (todo is null)
        {
            return false;
        }

        database.Todos.Remove(todo);
        await database.SaveChangesAsync(cancellationToken);
        return true;
    }
}
