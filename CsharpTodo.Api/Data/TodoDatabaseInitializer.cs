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

        if (await database.Todos.AnyAsync())
        {
            return;
        }

        database.Todos.AddRange(
            new TaskItem
            {
                Title = "Learn ASP.NET Core",
                Description = "Complete the Todo API tutorial.",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Title = "Run the API",
                Description = "Open Swagger and test the endpoints.",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            });

        await database.SaveChangesAsync();
    }
}
