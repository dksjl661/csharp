using CsharpTodo.Api.Contracts;
using CsharpTodo.Api.Data;
using CsharpTodo.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TodoDatabase")
    ?? throw new InvalidOperationException("Connection string 'TodoDatabase' is required.");

builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    await TodoDatabaseInitializer.InitializeAsync(database);
}

var todos = app.MapGroup("/api/todos").WithTags("Todos");

todos.MapGet("/", async (TodoDbContext database) =>
    Results.Ok(await database.Todos.OrderBy(todo => todo.Id).ToListAsync()));

todos.MapGet("/{id:int}", async (int id, TodoDbContext database) =>
{
    var todo = await database.Todos.FindAsync(id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
});

todos.MapPost("/", async (TodoRequest request, TodoDbContext database) =>
{
    if (!request.IsValid(out var error))
    {
        return Results.BadRequest(new { error });
    }

    var todo = new TaskItem
    {
        Title = request.Title!.Trim(),
        Description = request.Description,
        IsCompleted = request.IsCompleted,
        CreatedAt = DateTime.UtcNow
    };

    database.Todos.Add(todo);
    await database.SaveChangesAsync();

    return Results.Created($"/api/todos/{todo.Id}", todo);
});

todos.MapPut("/{id:int}", async (int id, TodoRequest request, TodoDbContext database) =>
{
    if (!request.IsValid(out var error))
    {
        return Results.BadRequest(new { error });
    }

    var todo = await database.Todos.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    todo.Title = request.Title!.Trim();
    todo.Description = request.Description;
    todo.IsCompleted = request.IsCompleted;
    await database.SaveChangesAsync();

    return Results.Ok(todo);
});

todos.MapDelete("/{id:int}", async (int id, TodoDbContext database) =>
{
    var todo = await database.Todos.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    database.Todos.Remove(todo);
    await database.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

public partial class Program;
