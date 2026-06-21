using CsharpTodo.Api.Application.Labels;
using CsharpTodo.Api.Application.Todos;
using Microsoft.AspNetCore.Mvc;

namespace CsharpTodo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LabelsController(GetLabelsQueryHandler getLabels) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LabelDto>>> GetLabels(CancellationToken cancellationToken)
    {
        return Ok(await getLabels.HandleAsync(cancellationToken));
    }
}
