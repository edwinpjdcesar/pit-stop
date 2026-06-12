using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class MaintenancePartService : IMaintenancePartService
{
    private readonly PitStopContext _context;

    public MaintenancePartService(PitStopContext context)
    {
        _context = context;
    }

    public async Task<MaintenancePartResponse> AddPartAsync(Guid maintenanceId, Guid partId, MaintenancePartRequest request, CancellationToken cancellationToken = default)
    {
        var maintenanceExists = await _context.MaintenanceRecords.AnyAsync(m => m.MaintenanceId == maintenanceId, cancellationToken);
        if (!maintenanceExists)
            throw new NotFoundException(nameof(Maintenance), maintenanceId);

        var part = await _context.Parts.FirstOrDefaultAsync(p => p.PartId == partId, cancellationToken)
            ?? throw new NotFoundException(nameof(Part), partId);

        var alreadyLinked = await _context.MaintenanceParts.AnyAsync(mp => mp.MaintenanceId == maintenanceId && mp.PartId == partId, cancellationToken);
        if (alreadyLinked)
            throw new InvalidOperationException($"Part with ID {partId} is already linked to Maintenance {maintenanceId}.");

        var maintenancePart = new MaintenancePart
        {
            MaintenanceId = maintenanceId,
            PartId = partId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice
        };

        _context.MaintenanceParts.Add(maintenancePart);
        await _context.SaveChangesAsync(cancellationToken);

        return new MaintenancePartResponse(maintenanceId, partId, part.Name, request.Quantity, request.UnitPrice);
    }

    public async Task<MaintenancePartResponse> UpdatePartAsync(Guid maintenanceId, Guid partId, MaintenancePartRequest request, CancellationToken cancellationToken = default)
    {
        var maintenancePart = await _context.MaintenanceParts
            .Include(mp => mp.Part)
            .FirstOrDefaultAsync(mp => mp.MaintenanceId == maintenanceId && mp.PartId == partId, cancellationToken)
            ?? throw new NotFoundException("MaintenancePart", $"{maintenanceId}/{partId}");

        maintenancePart.Quantity = request.Quantity;
        maintenancePart.UnitPrice = request.UnitPrice;

        await _context.SaveChangesAsync(cancellationToken);

        return new MaintenancePartResponse(maintenanceId, partId, maintenancePart.Part.Name, maintenancePart.Quantity, maintenancePart.UnitPrice);
    }

    public async Task RemovePartAsync(Guid maintenanceId, Guid partId, CancellationToken cancellationToken = default)
    {
        var maintenancePart = await _context.MaintenanceParts
            .FirstOrDefaultAsync(mp => mp.MaintenanceId == maintenanceId && mp.PartId == partId, cancellationToken)
            ?? throw new NotFoundException("MaintenancePart", $"{maintenanceId}/{partId}");

        _context.MaintenanceParts.Remove(maintenancePart);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
