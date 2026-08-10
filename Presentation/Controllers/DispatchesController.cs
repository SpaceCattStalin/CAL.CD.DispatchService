using Application.Dispatches;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("dispatch")]
public class DispatchesController : ControllerBase
{
    private readonly DispatchService _dispatchService;

    public DispatchesController(DispatchService dispatchService)
    {
        _dispatchService = dispatchService;
    }

    [HttpPost]
    // TODO: [Authorize(Roles = "Owner")] once authentication is implemented
    public async Task<IActionResult> Create(CreateDispatchRequest request)
    {
        var response = await _dispatchService.CreateAsync(request);
        return Created($"/dispatch/{response.DispatchId}", response);
    }
}
