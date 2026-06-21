using CsharpTodo.Api.Application.Todos;
using CsharpTodo.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CsharpTodo.Api.Application.Labels;

public sealed class GetLabelsQueryHandler(TodoDbContext database)
{
    public async Task<IReadOnlyList<LabelDto>> HandleAsync(CancellationToken cancellationToken)
    {
        return await database.Labels
            .AsNoTracking()
            .OrderBy(label => label.Name)
            .Select(label => new LabelDto(label.Id, label.Name, label.Description, label.Color))
            .ToListAsync(cancellationToken);
    }
}
