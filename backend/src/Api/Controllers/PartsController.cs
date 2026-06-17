using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>CRUD operations for parts.</summary>
[ApiController]
[Route("api/parts")]
[Produces("application/json")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    public PartsController(IPartService partService) => _partService = partService;

    /// <summary>Get all parts.</summary>
    /// <response code="200">List of parts.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PartResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var parts = await _partService.GetAllAsync(cancellationToken);
        return Ok(parts);
    }

    /// <summary>Get a part by ID.</summary>
    /// <param name="partId">The part ID.</param>
    /// <response code="200">The part.</response>
    /// <response code="404">Part not found.</response>
    [HttpGet("{partId:guid}")]
    [ProducesResponseType(typeof(PartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid partId, CancellationToken cancellationToken)
    {
        var part = await _partService.GetByIdAsync(partId, cancellationToken);
        if (part is null)
            return NotFound(new ErrorResponse(new ErrorDetail("NOT_FOUND", $"Part with ID {partId} was not found.")));

        return Ok(part);
    }

    /// <summary>Create a part.</summary>
    /// <response code="201">Part created.</response>
    /// <response code="400">Invalid request body.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PartResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PartRequest request, CancellationToken cancellationToken)
    {
        var part = await _partService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { partId = part.PartId }, part);
    }

    /// <summary>Update a part.</summary>
    /// <param name="partId">The part ID.</param>
    /// <response code="200">Updated part.</response>
    /// <response code="400">Invalid request body.</response>
    /// <response code="404">Part not found.</response>
    [HttpPut("{partId:guid}")]
    [ProducesResponseType(typeof(PartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid partId, [FromBody] PartRequest request, CancellationToken cancellationToken)
    {
        var part = await _partService.UpdateAsync(partId, request, cancellationToken);
        return Ok(part);
    }

    /// <summary>Delete a part and remove it from all maintenance records.</summary>
    /// <param name="partId">The part ID.</param>
    /// <response code="204">Part deleted.</response>
    /// <response code="404">Part not found.</response>
    [HttpDelete("{partId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid partId, CancellationToken cancellationToken)
    {
        await _partService.DeleteAsync(partId, cancellationToken);
        return NoContent();
    }
}
