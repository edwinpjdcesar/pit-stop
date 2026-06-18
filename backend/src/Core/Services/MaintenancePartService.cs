using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class MaintenancePartService(PitStopContext context, ILogger<MaintenancePartService> logger) : IMaintenancePartService
{
    private readonly PitStopContext _context = context;
    private readonly ILogger<MaintenancePartService> _logger = logger;

    public async Task<MaintenancePartResponse> AddPartAsync(Guid maintenanceId, Guid partId, MaintenancePartRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding part {PartId} to maintenance record {MaintenanceId}", partId, maintenanceId);

        var maintenanceExists = await _context.MaintenanceRecords.AnyAsync(m => m.MaintenanceId == maintenanceId, cancellationToken);
        if (!maintenanceExists)
        {
            _logger.LogWarning("Maintenance record {MaintenanceId} not found", maintenanceId);
            throw new NotFoundException(nameof(Maintenance), maintenanceId);
        }

        var part = await _context.Parts.FirstOrDefaultAsync(p => p.PartId == partId, cancellationToken);
        if (part is null)
        {
            _logger.LogWarning("Part {PartId} not found", partId);
            throw new NotFoundException(nameof(Part), partId);
        }

        var alreadyLinked = await _context.MaintenanceParts.AnyAsync(mp => mp.MaintenanceId == maintenanceId && mp.PartId == partId, cancellationToken);
        if (alreadyLinked)
        {
            _logger.LogWarning("Part {PartId} is already linked to maintenance record {MaintenanceId}", partId, maintenanceId);
            throw new InvalidOperationException($"Part with ID {partId} is already linked to Maintenance {maintenanceId}");
        }

        var maintenancePart = new MaintenancePart
        {
            MaintenanceId = maintenanceId,
            PartId = partId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice
        };

        _context.MaintenanceParts.Add(maintenancePart);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Added part {PartId} to maintenance record {MaintenanceId}", partId, maintenanceId);

        return new MaintenancePartResponse(maintenanceId, partId, part.Name, request.Quantity, request.UnitPrice);
    }

    public async Task<MaintenancePartResponse> UpdatePartAsync(Guid maintenanceId, Guid partId, MaintenancePartRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating part {PartId} on maintenance record {MaintenanceId}", partId, maintenanceId);

        var maintenancePart = await _context.MaintenanceParts
            .Include(mp => mp.Part)
            .FirstOrDefaultAsync(mp => mp.MaintenanceId == maintenanceId && mp.PartId == partId, cancellationToken);

        if (maintenancePart is null)
        {
            _logger.LogWarning("Maintenance part link {MaintenanceId}/{PartId} not found", maintenanceId, partId);
            throw new NotFoundException("MaintenancePart", $"{maintenanceId}/{partId}");
        }

        maintenancePart.Quantity = request.Quantity;
        maintenancePart.UnitPrice = request.UnitPrice;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated part {PartId} on maintenance record {MaintenanceId}", partId, maintenanceId);

        return new MaintenancePartResponse(maintenanceId, partId, maintenancePart.Part.Name, maintenancePart.Quantity, maintenancePart.UnitPrice);
    }

    public async Task RemovePartAsync(Guid maintenanceId, Guid partId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Removing part {PartId} from maintenance record {MaintenanceId}", partId, maintenanceId);

        var maintenancePart = await _context.MaintenanceParts
            .FirstOrDefaultAsync(mp => mp.MaintenanceId == maintenanceId && mp.PartId == partId, cancellationToken);

        if (maintenancePart is null)
        {
            _logger.LogWarning("Maintenance part link {MaintenanceId}/{PartId} not found", maintenanceId, partId);
            throw new NotFoundException("MaintenancePart", $"{maintenanceId}/{partId}");
        }

        _context.MaintenanceParts.Remove(maintenancePart);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Removed part {PartId} from maintenance record {MaintenanceId}", partId, maintenanceId);
    }
}
