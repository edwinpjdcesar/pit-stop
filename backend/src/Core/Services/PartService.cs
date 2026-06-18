using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class PartService(PitStopContext context, ILogger<PartService> logger) : IPartService
{
    private readonly PitStopContext _context = context;
    private readonly ILogger<PartService> _logger = logger;

    public async Task<IReadOnlyList<PartResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all parts");

        var parts = await _context.Parts
            .OrderBy(p => p.Name)
            .Select(p => new PartResponse(p.PartId, p.Name, p.ModelNumber, p.Description))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} parts", parts.Count);

        return parts;
    }

    public async Task<PartResponse?> GetByIdAsync(Guid partId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting part {PartId}", partId);

        var part = await _context.Parts
            .FirstOrDefaultAsync(p => p.PartId == partId, cancellationToken);

        if (part is null)
            _logger.LogWarning("Part {PartId} not found", partId);

        return part is null ? null : ToResponse(part);
    }

    public async Task<PartResponse> CreateAsync(PartRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating part '{Name}'", request.Name);

        var part = new Part
        {
            PartId = Guid.NewGuid(),
            Name = request.Name,
            ModelNumber = request.ModelNumber,
            Description = request.Description
        };

        _context.Parts.Add(part);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created part {PartId}", part.PartId);

        return ToResponse(part);
    }

    public async Task<PartResponse> UpdateAsync(Guid partId, PartRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating part {PartId}", partId);

        var part = await _context.Parts
            .FirstOrDefaultAsync(p => p.PartId == partId, cancellationToken);

        if (part is null)
        {
            _logger.LogWarning("Part {PartId} not found", partId);
            throw new NotFoundException(nameof(Part), partId);
        }

        part.Name = request.Name;
        part.ModelNumber = request.ModelNumber;
        part.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated part {PartId}", partId);

        return ToResponse(part);
    }

    public async Task DeleteAsync(Guid partId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting part {PartId}", partId);

        var part = await _context.Parts
            .FirstOrDefaultAsync(p => p.PartId == partId, cancellationToken);

        if (part is null)
        {
            _logger.LogWarning("Part {PartId} not found", partId);
            throw new NotFoundException(nameof(Part), partId);
        }

        _context.Parts.Remove(part);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted part {PartId}", partId);
    }

    private static PartResponse ToResponse(Part p) => new(p.PartId, p.Name, p.ModelNumber, p.Description);
}
