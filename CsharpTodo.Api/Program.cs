using CsharpTodo.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TodoDatabase")
    ?? throw new InvalidOperationException("Connection string 'TodoDatabase' is required.");

builder.Services.AddControllers();
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

app.MapControllers();
app.Run();

public partial class Program;
