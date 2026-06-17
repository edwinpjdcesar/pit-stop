using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Parts linked to a specific maintenance record.</summary>
[ApiController]
[Route("api/vehicles/{vehicleId:guid}/maintenance/{maintenanceId:guid}/parts")]
[Produces("application/json")]
public class MaintenancePartsController : ControllerBase
{
    private readonly IMaintenancePartService _maintenancePartService;
    private readonly IMaintenanceService _maintenanceService;

    public MaintenancePartsController(
        IMaintenancePartService maintenancePartService,
        IMaintenanceService maintenanceService)
    {
        _maintenancePartService = maintenancePartService;
        _maintenanceService = maintenanceService;
    }

    /// <summary>Add an existing part to a maintenance record.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <param name="maintenanceId">The maintenance record ID.</param>
    /// <param name="partId">The part ID.</param>
    /// <response code="201">Part linked to maintenance record.</response>
    /// <response code="400">Invalid request body.</response>
    /// <response code="404">Vehicle, maintenance record, or part not found.</response>
    /// <response code="409">Maintenance record does not belong to the vehicle, or part is already linked.</response>
    [HttpPost("{partId:guid}")]
    [ProducesResponseType(typeof(MaintenancePartResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddPart(
        Guid vehicleId,
        Guid maintenanceId,
        Guid partId,
        [FromBody] MaintenancePartRequest request,
        CancellationToken cancellationToken)
    {
        await _maintenanceService.ValidateOwnershipAsync(vehicleId, maintenanceId, cancellationToken);
        var result = await _maintenancePartService.AddPartAsync(maintenanceId, partId, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Update quantity and unit price for a part on a maintenance record.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <param name="maintenanceId">The maintenance record ID.</param>
    /// <param name="partId">The part ID.</param>
    /// <response code="200">Updated maintenance part link.</response>
    /// <response code="400">Invalid request body.</response>
    /// <response code="404">Vehicle, maintenance record, part, or link not found.</response>
    /// <response code="409">Maintenance record does not belong to the vehicle.</response>
    [HttpPut("{partId:guid}")]
    [ProducesResponseType(typeof(MaintenancePartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdatePart(
        Guid vehicleId,
        Guid maintenanceId,
        Guid partId,
        [FromBody] MaintenancePartRequest request,
        CancellationToken cancellationToken)
    {
        await _maintenanceService.ValidateOwnershipAsync(vehicleId, maintenanceId, cancellationToken);
        var result = await _maintenancePartService.UpdatePartAsync(maintenanceId, partId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Remove a part from a maintenance record without deleting the part itself.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <param name="maintenanceId">The maintenance record ID.</param>
    /// <param name="partId">The part ID.</param>
    /// <response code="204">Part removed from maintenance record.</response>
    /// <response code="404">Vehicle, maintenance record, part, or link not found.</response>
    /// <response code="409">Maintenance record does not belong to the vehicle.</response>
    [HttpDelete("{partId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemovePart(
        Guid vehicleId,
        Guid maintenanceId,
        Guid partId,
        CancellationToken cancellationToken)
    {
        await _maintenanceService.ValidateOwnershipAsync(vehicleId, maintenanceId, cancellationToken);
        await _maintenancePartService.RemovePartAsync(maintenanceId, partId, cancellationToken);
        return NoContent();
    }
}
