using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CsharpTodo.Tests;

public class TodoApiTests(TodoApiFactory factory) : IClassFixture<TodoApiFactory>
{
    [Fact]
    public void Todo_routes_are_served_by_todos_controller()
    {
        using var scope = factory.Services.CreateScope();
        var descriptions = scope.ServiceProvider
            .GetRequiredService<IApiDescriptionGroupCollectionProvider>()
            .ApiDescriptionGroups
            .Items
            .SelectMany(group => group.Items);

        var route = Assert.Single(descriptions, description =>
            description.HttpMethod == HttpMethod.Get.Method
            && string.Equals(description.RelativePath?.TrimEnd('/'), "api/todos", StringComparison.OrdinalIgnoreCase));

        var action = Assert.IsType<ControllerActionDescriptor>(route.ActionDescriptor);
        Assert.Equal("TodosController", action.ControllerTypeInfo.Name);
    }

    [Fact]
    public async Task Get_todos_returns_seeded_items()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/todos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        Assert.Contains(todos!, todo => todo.Title == "Learn ASP.NET Core");
    }

    [Fact]
    public async Task Get_todos_includes_the_assigned_label()
    {
        using var client = factory.CreateClient();

        var todos = await client.GetFromJsonAsync<List<TodoResponse>>("/api/todos");
        var studyTodo = Assert.Single(todos!, todo => todo.Title == "Learn ASP.NET Core");

        Assert.NotNull(studyTodo.Label);
        Assert.Equal("Study", studyTodo.Label.Name);
        Assert.Equal("#2563EB", studyTodo.Label.Color);
    }

    [Fact]
    public async Task Get_labels_returns_seeded_labels()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/labels");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var labels = await response.Content.ReadFromJsonAsync<List<LabelResponse>>();
        Assert.Contains(labels!, label => label.Name == "Work" && label.Color == "#F97316");
    }

    [Fact]
    public async Task Get_todo_returns_not_found_for_unknown_id()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/todos/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_todo_creates_a_todo()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/todos", new
        {
            title = "Write integration tests",
            description = "Cover the Todo API"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(todo);
        Assert.Equal("Write integration tests", todo.Title);
        Assert.Equal("Cover the Todo API", todo.Description);
        Assert.False(todo.IsCompleted);
        Assert.NotEqual(default, todo.CreatedAt);
    }

    [Fact]
    public async Task Put_todo_updates_an_existing_todo()
    {
        using var client = factory.CreateClient();
        var created = await CreateTodo(client);

        var response = await client.PutAsJsonAsync($"/api/todos/{created.Id}", new
        {
            title = "Updated title",
            description = "Updated description",
            isCompleted = true
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(updated);
        Assert.Equal("Updated title", updated.Title);
        Assert.Equal("Updated description", updated.Description);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public async Task Delete_todo_removes_an_existing_todo()
    {
        using var client = factory.CreateClient();
        var created = await CreateTodo(client);

        var deleteResponse = await client.DeleteAsync($"/api/todos/{created.Id}");
        var getResponse = await client.GetAsync($"/api/todos/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_todo_rejects_an_empty_title()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/todos", new { title = " " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<TodoResponse> CreateTodo(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/todos", new { title = "Temporary todo" });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<TodoResponse>())!;
    }

    private sealed record TodoResponse(
        int Id,
        string Title,
        string? Description,
        bool IsCompleted,
        DateTime CreatedAt,
        LabelResponse? Label);

    private sealed record LabelResponse(int Id, string Name, string? Description, string Color);
}
