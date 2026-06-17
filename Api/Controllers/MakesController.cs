using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Vehicle makes (read-only reference data).</summary>
[ApiController]
[Route("api/makes")]
[Produces("application/json")]
public class MakesController : ControllerBase
{
    private readonly IMakeService _makeService;

    public MakesController(IMakeService makeService) => _makeService = makeService;

    /// <summary>Get all vehicle makes.</summary>
    /// <response code="200">List of makes.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MakeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var makes = await _makeService.GetAllAsync(cancellationToken);
        return Ok(makes);
    }
}
