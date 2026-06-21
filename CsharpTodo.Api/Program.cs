using CsharpTodo.Api.Data;
using CsharpTodo.Api.Application.Labels;
using CsharpTodo.Api.Application.Todos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TodoDatabase")
    ?? throw new InvalidOperationException("Connection string 'TodoDatabase' is required.");

builder.Services.AddControllers();
builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<GetTodosQueryHandler>();
builder.Services.AddScoped<GetTodoQueryHandler>();
builder.Services.AddScoped<CreateTodoCommandHandler>();
builder.Services.AddScoped<UpdateTodoCommandHandler>();
builder.Services.AddScoped<DeleteTodoCommandHandler>();
builder.Services.AddScoped<GetLabelsQueryHandler>();
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"])
    .AllowAnyHeader()
    .AllowAnyMethod()));
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

app.UseCors("Frontend");
app.MapControllers();
app.Run();

public partial class Program;
