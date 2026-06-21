using CsharpTodo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CsharpTodo.Api.Data;

public static class TodoDatabaseInitializer
{
    public static async Task InitializeAsync(TodoDbContext database)
    {
        if (database.Database.IsRelational())
        {
            await database.Database.MigrateAsync();
        }
        else
        {
            await database.Database.EnsureCreatedAsync();
        }

        if (!await database.Labels.AnyAsync())
        {
            database.Labels.AddRange(
                new Label { Name = "Study", Description = "Focused learning work.", Color = "#2563EB" },
                new Label { Name = "Work", Description = "Professional tasks.", Color = "#F97316" },
                new Label { Name = "Personal", Description = "Personal priorities.", Color = "#7C3AED" });
            await database.SaveChangesAsync();
        }

        var labels = await database.Labels.ToDictionaryAsync(label => label.Name);

        if (!await database.Todos.AnyAsync())
        {
            database.Todos.AddRange(
                new TaskItem
                {
                    Title = "Learn ASP.NET Core",
                    Description = "Complete the Todo API tutorial.",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    LabelId = labels["Study"].Id
                },
                new TaskItem
                {
                    Title = "Run the API",
                    Description = "Open Swagger and test the endpoints.",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    LabelId = labels["Work"].Id
                });
        }
        else
        {
            var todos = await database.Todos.Where(todo => todo.LabelId == null).ToListAsync();
            foreach (var todo in todos)
            {
                todo.LabelId = todo.Title == "Learn ASP.NET Core" ? labels["Study"].Id : labels["Work"].Id;
            }
        }

        await database.SaveChangesAsync();
    }
}
