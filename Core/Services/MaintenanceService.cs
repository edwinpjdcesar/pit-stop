using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly PitStopContext _context;

    public MaintenanceService(PitStopContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<MaintenanceResponse>> GetByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var vehicleExists = await _context.Vehicles.AnyAsync(v => v.VehicleId == vehicleId, cancellationToken);
        if (!vehicleExists)
            throw new NotFoundException(nameof(Vehicle), vehicleId);

        var records = await _context.MaintenanceRecords
            .Where(m => m.VehicleId == vehicleId)
            .Include(m => m.MaintenanceParts)
                .ThenInclude(mp => mp.Part)
            .OrderByDescending(m => m.ServiceDate)
            .ToListAsync(cancellationToken);

        return records.Select(ToResponse).ToList();
    }

    public async Task<MaintenanceResponse> AddAsync(Guid vehicleId, MaintenanceRequest request, CancellationToken cancellationToken = default)
    {
        var vehicleExists = await _context.Vehicles.AnyAsync(v => v.VehicleId == vehicleId, cancellationToken);
        if (!vehicleExists)
            throw new NotFoundException(nameof(Vehicle), vehicleId);

        var maintenance = new Maintenance
        {
            MaintenanceId = Guid.NewGuid(),
            VehicleId = vehicleId,
            Description = request.Description,
            Mileage = request.Mileage,
            ServiceDate = request.ServiceDate
        };

        _context.MaintenanceRecords.Add(maintenance);
        await _context.SaveChangesAsync(cancellationToken);

        return ToResponse(maintenance);
    }

    public async Task<MaintenanceResponse> UpdateAsync(Guid vehicleId, Guid maintenanceId, MaintenanceRequest request, CancellationToken cancellationToken = default)
    {
        var maintenance = await _context.MaintenanceRecords
            .Include(m => m.MaintenanceParts)
                .ThenInclude(mp => mp.Part)
            .FirstOrDefaultAsync(m => m.MaintenanceId == maintenanceId && m.VehicleId == vehicleId, cancellationToken)
            ?? throw new NotFoundException(nameof(Maintenance), maintenanceId);

        maintenance.Description = request.Description;
        maintenance.Mileage = request.Mileage;
        maintenance.ServiceDate = request.ServiceDate;

        await _context.SaveChangesAsync(cancellationToken);

        return ToResponse(maintenance);
    }

    public async Task DeleteAsync(Guid vehicleId, Guid maintenanceId, CancellationToken cancellationToken = default)
    {
        var maintenance = await _context.MaintenanceRecords
            .FirstOrDefaultAsync(m => m.MaintenanceId == maintenanceId && m.VehicleId == vehicleId, cancellationToken)
            ?? throw new NotFoundException(nameof(Maintenance), maintenanceId);

        _context.MaintenanceRecords.Remove(maintenance);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ValidateOwnershipAsync(Guid vehicleId, Guid maintenanceId, CancellationToken cancellationToken = default)
    {
        var maintenance = await _context.MaintenanceRecords
            .FirstOrDefaultAsync(m => m.MaintenanceId == maintenanceId, cancellationToken)
            ?? throw new NotFoundException(nameof(Maintenance), maintenanceId);

        if (maintenance.VehicleId != vehicleId)
            throw new ConflictException($"Maintenance {maintenanceId} does not belong to vehicle {vehicleId}.");
    }

    private static MaintenanceResponse ToResponse(Maintenance m) => new(
        m.MaintenanceId,
        m.VehicleId,
        m.Description,
        m.Mileage,
        m.ServiceDate,
        m.MaintenanceParts.Select(mp => new MaintenancePartResponse(
            mp.MaintenanceId,
            mp.PartId,
            mp.Part.Name,
            mp.Quantity,
            mp.UnitPrice
        )).ToList()
    );
}
