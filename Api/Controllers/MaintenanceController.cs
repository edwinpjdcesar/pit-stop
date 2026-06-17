using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Maintenance records scoped to a vehicle.</summary>
[ApiController]
[Route("api/vehicles/{vehicleId:guid}/maintenance")]
[Produces("application/json")]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _maintenanceService;

    public MaintenanceController(IMaintenanceService maintenanceService) => _maintenanceService = maintenanceService;

    /// <summary>Get all maintenance records for a vehicle.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <response code="200">List of maintenance records.</response>
    /// <response code="404">Vehicle not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MaintenanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId, CancellationToken cancellationToken)
    {
        var records = await _maintenanceService.GetByVehicleAsync(vehicleId, cancellationToken);
        return Ok(records);
    }

    /// <summary>Add a maintenance record to a vehicle.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <response code="201">Maintenance record created.</response>
    /// <response code="400">Invalid request body.</response>
    /// <response code="404">Vehicle not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MaintenanceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add(Guid vehicleId, [FromBody] MaintenanceRequest request, CancellationToken cancellationToken)
    {
        var maintenance = await _maintenanceService.AddAsync(vehicleId, request, cancellationToken);
        return CreatedAtAction(nameof(GetByVehicle), new { vehicleId }, maintenance);
    }

    /// <summary>Update a maintenance record.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <param name="maintenanceId">The maintenance record ID.</param>
    /// <response code="200">Updated maintenance record.</response>
    /// <response code="400">Invalid request body.</response>
    /// <response code="404">Vehicle or maintenance record not found.</response>
    /// <response code="409">Maintenance record does not belong to the vehicle.</response>
    [HttpPut("{maintenanceId:guid}")]
    [ProducesResponseType(typeof(MaintenanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid vehicleId, Guid maintenanceId, [FromBody] MaintenanceRequest request, CancellationToken cancellationToken)
    {
        var maintenance = await _maintenanceService.UpdateAsync(vehicleId, maintenanceId, request, cancellationToken);
        return Ok(maintenance);
    }

    /// <summary>Delete a maintenance record from a vehicle.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <param name="maintenanceId">The maintenance record ID.</param>
    /// <response code="204">Maintenance record deleted.</response>
    /// <response code="404">Vehicle or maintenance record not found.</response>
    /// <response code="409">Maintenance record does not belong to the vehicle.</response>
    [HttpDelete("{maintenanceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid vehicleId, Guid maintenanceId, CancellationToken cancellationToken)
    {
        await _maintenanceService.DeleteAsync(vehicleId, maintenanceId, cancellationToken);
        return NoContent();
    }
}
