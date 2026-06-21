namespace CsharpTodo.Api.Application.Todos;

public sealed record TodoInput(string? Title, string? Description, bool IsCompleted, int? LabelId)
{
    public bool IsValid(out string? error)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            error = "Title is required.";
            return false;
        }

        if (Title.Length > 200)
        {
            error = "Title must be 200 characters or fewer.";
            return false;
        }

        if (Description?.Length > 1_000)
        {
            error = "Description must be 1000 characters or fewer.";
            return false;
        }

        error = null;
        return true;
    }
}
