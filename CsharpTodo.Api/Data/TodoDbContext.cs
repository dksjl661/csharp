using CsharpTodo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CsharpTodo.Api.Data;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Todos => Set<TaskItem>();

    public DbSet<Label> Labels => Set<Label>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(todo => todo.Title).HasMaxLength(200).IsRequired();
            entity.Property(todo => todo.Description).HasMaxLength(1_000);
            entity.HasOne(todo => todo.Label)
                .WithMany(label => label.Todos)
                .HasForeignKey(todo => todo.LabelId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.Property(label => label.Name).HasMaxLength(100).IsRequired();
            entity.Property(label => label.Description).HasMaxLength(500);
            entity.Property(label => label.Color).HasMaxLength(20).IsRequired();
            entity.HasIndex(label => label.Name).IsUnique();
        });
    }
}
