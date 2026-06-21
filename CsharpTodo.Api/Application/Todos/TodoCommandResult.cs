namespace CsharpTodo.Api.Application.Todos;

public sealed record TodoCommandResult(TodoDto? Todo, string? Error = null, bool NotFound = false)
{
    public static TodoCommandResult MissingTodo() => new(null, NotFound: true);

    public static TodoCommandResult InvalidLabel() => new(null, "The selected label does not exist.");
}
