using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class VehicleService(PitStopContext context, ILogger<VehicleService> logger) : IVehicleService
{
    private readonly PitStopContext _context = context;
    private readonly ILogger<VehicleService> _logger = logger;

    public async Task<IReadOnlyList<VehicleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all vehicles");

        var vehicles = await _context.Vehicles
            .Include(v => v.Make)
            .Include(v => v.Model)
            .OrderBy(v => v.Year)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} vehicles", vehicles.Count);

        return vehicles.Select(ToResponse).ToList();
    }

    public async Task<VehicleResponse?> GetByIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting vehicle {VehicleId}", vehicleId);

        var vehicle = await _context.Vehicles
            .Include(v => v.Make)
            .Include(v => v.Model)
            .FirstOrDefaultAsync(v => v.VehicleId == vehicleId, cancellationToken);

        if (vehicle is null)
            _logger.LogWarning("Vehicle {VehicleId} not found", vehicleId);

        return vehicle is null ? null : ToResponse(vehicle);
    }

    public async Task<VehicleResponse> CreateAsync(VehicleRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating vehicle for make {MakeId}, model {ModelId}", request.MakeId, request.ModelId);

        await ValidateMakeAndModelAsync(request.MakeId, request.ModelId, cancellationToken);

        var vehicle = new Vehicle
        {
            VehicleId = Guid.NewGuid(),
            MakeId = request.MakeId,
            ModelId = request.ModelId,
            VIN = request.VIN,
            LicensePlate = request.LicensePlate,
            Year = request.Year,
            PurchaseDate = request.PurchaseDate,
            PurchasePrice = request.PurchasePrice,
            MileageAtPurchase = request.MileageAtPurchase,
            Mileage = request.Mileage
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(vehicle).Reference(v => v.Make).LoadAsync(cancellationToken);
        await _context.Entry(vehicle).Reference(v => v.Model).LoadAsync(cancellationToken);

        _logger.LogInformation("Created vehicle {VehicleId}", vehicle.VehicleId);

        return ToResponse(vehicle);
    }

    public async Task<VehicleResponse> UpdateAsync(Guid vehicleId, VehicleRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating vehicle {VehicleId}", vehicleId);

        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == vehicleId, cancellationToken);

        if (vehicle is null)
        {
            _logger.LogWarning("Vehicle {VehicleId} not found", vehicleId);
            throw new NotFoundException(nameof(Vehicle), vehicleId);
        }

        await ValidateMakeAndModelAsync(request.MakeId, request.ModelId, cancellationToken);

        vehicle.MakeId = request.MakeId;
        vehicle.ModelId = request.ModelId;
        vehicle.VIN = request.VIN;
        vehicle.LicensePlate = request.LicensePlate;
        vehicle.Year = request.Year;
        vehicle.PurchaseDate = request.PurchaseDate;
        vehicle.PurchasePrice = request.PurchasePrice;
        vehicle.MileageAtPurchase = request.MileageAtPurchase;
        vehicle.Mileage = request.Mileage;

        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(vehicle).Reference(v => v.Make).LoadAsync(cancellationToken);
        await _context.Entry(vehicle).Reference(v => v.Model).LoadAsync(cancellationToken);

        _logger.LogInformation("Updated vehicle {VehicleId}", vehicleId);

        return ToResponse(vehicle);
    }

    public async Task DeleteAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting vehicle {VehicleId}", vehicleId);

        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == vehicleId, cancellationToken);

        if (vehicle is null)
        {
            _logger.LogWarning("Vehicle {VehicleId} not found", vehicleId);
            throw new NotFoundException(nameof(Vehicle), vehicleId);
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted vehicle {VehicleId}", vehicleId);
    }

    private async Task ValidateMakeAndModelAsync(int makeId, int modelId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating model {ModelId} belongs to make {MakeId}", modelId, makeId);

        var makeExists = await _context.Makes.AnyAsync(m => m.MakeId == makeId, cancellationToken);
        if (!makeExists)
        {
            _logger.LogWarning("Make {MakeId} not found", makeId);
            throw new NotFoundException(nameof(Make), makeId);
        }

        var model = await _context.Models.FirstOrDefaultAsync(m => m.ModelId == modelId, cancellationToken);
        if (model is null)
        {
            _logger.LogWarning("Model {ModelId} not found", modelId);
            throw new NotFoundException(nameof(Model), modelId);
        }

        if (model.MakeId != makeId)
        {
            _logger.LogWarning("Model {ModelId} does not belong to make {MakeId}", modelId, makeId);
            throw new ConflictException($"Model {modelId} does not belong to make {makeId}");
        }
    }

    private static VehicleResponse ToResponse(Vehicle v) => new(
        v.VehicleId,
        v.VIN,
        v.LicensePlate,
        v.Year,
        new MakeResponse(v.Make.MakeId, v.Make.Code, v.Make.Name),
        new ModelResponse(v.Model.ModelId, v.Model.MakeId, v.Make.Name, v.Model.Code, v.Model.Name),
        v.PurchaseDate,
        v.PurchasePrice,
        v.MileageAtPurchase,
        v.Mileage
    );
}
