using CsharpTodo.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CsharpTodo.Tests;

public sealed class TodoApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"todos-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<TodoDbContext>>();

            services.AddDbContext<TodoDbContext>(db =>
                db.UseInMemoryDatabase(_databaseName));
        });
    }
}
