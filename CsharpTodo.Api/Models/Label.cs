namespace CsharpTodo.Api.Models;

public class Label
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public required string Color { get; set; }

    public ICollection<TaskItem> Todos { get; } = new List<TaskItem>();
}
