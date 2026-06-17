using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>CRUD operations for vehicles.</summary>
[ApiController]
[Route("api/vehicles")]
[Produces("application/json")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService) => _vehicleService = vehicleService;

    /// <summary>Get all vehicles.</summary>
    /// <response code="200">List of vehicles.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VehicleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleService.GetAllAsync(cancellationToken);
        return Ok(vehicles);
    }

    /// <summary>Get a vehicle by ID.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <response code="200">The vehicle.</response>
    /// <response code="404">Vehicle not found.</response>
    [HttpGet("{vehicleId:guid}")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid vehicleId, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleService.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle is null)
            return NotFound(new ErrorResponse(new ErrorDetail("NOT_FOUND", $"Vehicle with ID {vehicleId} was not found.")));

        return Ok(vehicle);
    }

    /// <summary>Create a vehicle.</summary>
    /// <response code="201">Vehicle created.</response>
    /// <response code="400">Invalid request body.</response>
    /// <response code="404">Make or model not found.</response>
    /// <response code="409">Model does not belong to the specified make.</response>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] VehicleRequest request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { vehicleId = vehicle.VehicleId }, vehicle);
    }

    /// <summary>Update a vehicle.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <response code="200">Updated vehicle.</response>
    /// <response code="400">Invalid request body.</response>
    /// <response code="404">Vehicle, make, or model not found.</response>
    /// <response code="409">Model does not belong to the specified make.</response>
    [HttpPut("{vehicleId:guid}")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid vehicleId, [FromBody] VehicleRequest request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleService.UpdateAsync(vehicleId, request, cancellationToken);
        return Ok(vehicle);
    }

    /// <summary>Delete a vehicle and all of its maintenance data.</summary>
    /// <param name="vehicleId">The vehicle ID.</param>
    /// <response code="204">Vehicle deleted.</response>
    /// <response code="404">Vehicle not found.</response>
    [HttpDelete("{vehicleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid vehicleId, CancellationToken cancellationToken)
    {
        await _vehicleService.DeleteAsync(vehicleId, cancellationToken);
        return NoContent();
    }
}
