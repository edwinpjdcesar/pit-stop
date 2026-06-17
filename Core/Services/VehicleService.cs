using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class VehicleService : IVehicleService
{
    private readonly PitStopContext _context;

    public VehicleService(PitStopContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<VehicleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var vehicles = await _context.Vehicles
            .Include(v => v.Make)
            .Include(v => v.Model)
            .OrderBy(v => v.Year)
            .ToListAsync(cancellationToken);

        return vehicles.Select(ToResponse).ToList();
    }

    public async Task<VehicleResponse?> GetByIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Make)
            .Include(v => v.Model)
            .FirstOrDefaultAsync(v => v.VehicleId == vehicleId, cancellationToken);

        return vehicle is null ? null : ToResponse(vehicle);
    }

    public async Task<VehicleResponse> CreateAsync(VehicleRequest request, CancellationToken cancellationToken = default)
    {
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

        return ToResponse(vehicle);
    }

    public async Task<VehicleResponse> UpdateAsync(Guid vehicleId, VehicleRequest request, CancellationToken cancellationToken = default)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == vehicleId, cancellationToken)
            ?? throw new NotFoundException(nameof(Vehicle), vehicleId);

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

        return ToResponse(vehicle);
    }

    public async Task DeleteAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == vehicleId, cancellationToken)
            ?? throw new NotFoundException(nameof(Vehicle), vehicleId);

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateMakeAndModelAsync(int makeId, int modelId, CancellationToken cancellationToken)
    {
        var makeExists = await _context.Makes.AnyAsync(m => m.MakeId == makeId, cancellationToken);
        if (!makeExists)
            throw new NotFoundException(nameof(Make), makeId);

        var model = await _context.Models.FirstOrDefaultAsync(m => m.ModelId == modelId, cancellationToken);
        if (model is null)
            throw new NotFoundException(nameof(Model), modelId);

        if (model.MakeId != makeId)
            throw new ConflictException($"Model {modelId} does not belong to make {makeId}.");
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
