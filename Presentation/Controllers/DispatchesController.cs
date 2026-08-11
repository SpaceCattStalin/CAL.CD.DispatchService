using Application;
using Application.Dispatches;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = PermissionNames.DispatchesCreate)]
    public async Task<IActionResult> Create(CreateDispatchRequest request)
    {
        var response = await _dispatchService.CreateAsync(request);
        return Created($"/dispatch/{response.DispatchId}", response);
    }

    [HttpGet("{dispatchId}")]
    [Authorize(Policy = PermissionNames.DispatchesRead)]
    public async Task<IActionResult> GetById(Guid dispatchId)
    {
        var response = await _dispatchService.GetByIdAsync(dispatchId);
        return Ok(response);
    }

    [HttpPost("batch/get")]
    [Authorize(Policy = PermissionNames.DispatchesRead)]
    public async Task<IActionResult> GetBatch(GetDispatchBatchRequest request)
    {
        var response = await _dispatchService.GetBatchAsync(request);
        return Ok(response);
    }

    [HttpPost("{dispatchId}/assign-driver")]
    [Authorize(Policy = PermissionNames.DispatchesUpdate)]
    public async Task<IActionResult> AssignDriver(Guid dispatchId, AssignDriverRequest request)
    {
        await _dispatchService.AssignDriverAsync(dispatchId, request);
        return NoContent();
    }
}
