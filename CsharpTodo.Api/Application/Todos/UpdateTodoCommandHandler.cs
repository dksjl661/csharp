using CsharpTodo.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CsharpTodo.Api.Application.Todos;

public sealed class UpdateTodoCommandHandler(TodoDbContext database)
{
    public async Task<TodoCommandResult> HandleAsync(int id, TodoInput input, CancellationToken cancellationToken)
    {
        var todo = await database.Todos.FindAsync([id], cancellationToken);
        if (todo is null)
        {
            return TodoCommandResult.MissingTodo();
        }

        if (input.LabelId is not null && !await database.Labels.AnyAsync(label => label.Id == input.LabelId, cancellationToken))
        {
            return TodoCommandResult.InvalidLabel();
        }

        todo.Title = input.Title!.Trim();
        todo.Description = input.Description;
        todo.IsCompleted = input.IsCompleted;
        todo.LabelId = input.LabelId;
        await database.SaveChangesAsync(cancellationToken);
        await database.Entry(todo).Reference(item => item.Label).LoadAsync(cancellationToken);

        return new TodoCommandResult(TodoDto.From(todo));
    }
}
