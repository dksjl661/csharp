using CsharpTodo.Api.Models;

namespace CsharpTodo.Api.Application.Todos;

public sealed record TodoDto(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime CreatedAt,
    LabelDto? Label)
{
    public static TodoDto From(TaskItem todo) => new(
        todo.Id,
        todo.Title,
        todo.Description,
        todo.IsCompleted,
        todo.CreatedAt,
        todo.Label is null ? null : LabelDto.From(todo.Label));
}

public sealed record LabelDto(int Id, string Name, string? Description, string Color)
{
    public static LabelDto From(Label label) => new(label.Id, label.Name, label.Description, label.Color);
}
