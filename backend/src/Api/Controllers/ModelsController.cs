using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Vehicle models scoped to a make (read-only reference data).</summary>
[ApiController]
[Route("api/makes")]
[Produces("application/json")]
public class ModelsController : ControllerBase
{
    private readonly IModelService _modelService;

    public ModelsController(IModelService modelService) => _modelService = modelService;

    /// <summary>Get all models for a specific make.</summary>
    /// <param name="makeId">The make ID.</param>
    /// <response code="200">List of models for the make.</response>
    /// <response code="404">Make not found.</response>
    [HttpGet("{makeId}/models")]
    [ProducesResponseType(typeof(IReadOnlyList<ModelResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByMake(int makeId, CancellationToken cancellationToken)
    {
        var models = await _modelService.GetByMakeAsync(makeId, cancellationToken);
        return Ok(models);
    }
}
