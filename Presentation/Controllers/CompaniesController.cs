using Application;
using Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/company")]
public class CompaniesController : ControllerBase
{
    private readonly CompanyService _companyService;

    public CompaniesController(CompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("carriers")]
    [Authorize(Policy = PermissionNames.CompaniesRead)]
    public async Task<IActionResult> GetCarriers()
    {
        var response = await _companyService.GetCarriersAsync();
        return Ok(response);
    }

    [HttpGet("drivers")]
    [Authorize(Policy = PermissionNames.DriversRead)]
    public async Task<IActionResult> GetDrivers()
    {
        var response = await _companyService.GetDriversAsync();
        return Ok(response);
    }
}