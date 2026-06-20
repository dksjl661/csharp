using CsharpTodo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CsharpTodo.Api.Data;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Todos => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(todo => todo.Title).HasMaxLength(200).IsRequired();
            entity.Property(todo => todo.Description).HasMaxLength(1_000);
        });
    }
}
