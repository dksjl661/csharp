using CsharpTodo.Api.Data;
using CsharpTodo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CsharpTodo.Api.Application.Todos;

public sealed class CreateTodoCommandHandler(TodoDbContext database)
{
    public async Task<TodoCommandResult> HandleAsync(TodoInput input, CancellationToken cancellationToken)
    {
        if (input.LabelId is not null && !await database.Labels.AnyAsync(label => label.Id == input.LabelId, cancellationToken))
        {
            return TodoCommandResult.InvalidLabel();
        }

        var todo = new TaskItem
        {
            Title = input.Title!.Trim(),
            Description = input.Description,
            IsCompleted = input.IsCompleted,
            CreatedAt = DateTime.UtcNow,
            LabelId = input.LabelId
        };

        database.Todos.Add(todo);
        await database.SaveChangesAsync(cancellationToken);
        await database.Entry(todo).Reference(item => item.Label).LoadAsync(cancellationToken);

        return new TodoCommandResult(TodoDto.From(todo));
    }
}
