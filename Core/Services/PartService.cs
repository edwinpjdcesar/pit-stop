using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class PartService : IPartService
{
    private readonly PitStopContext _context;

    public PartService(PitStopContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PartResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .OrderBy(p => p.Name)
            .Select(p => new PartResponse(p.PartId, p.Name, p.ModelNumber, p.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<PartResponse?> GetByIdAsync(Guid partId, CancellationToken cancellationToken = default)
    {
        var part = await _context.Parts
            .FirstOrDefaultAsync(p => p.PartId == partId, cancellationToken);

        return part is null ? null : ToResponse(part);
    }

    public async Task<PartResponse> CreateAsync(PartRequest request, CancellationToken cancellationToken = default)
    {
        var part = new Part
        {
            PartId = Guid.NewGuid(),
            Name = request.Name,
            ModelNumber = request.ModelNumber,
            Description = request.Description
        };

        _context.Parts.Add(part);
        await _context.SaveChangesAsync(cancellationToken);

        return ToResponse(part);
    }

    public async Task<PartResponse> UpdateAsync(Guid partId, PartRequest request, CancellationToken cancellationToken = default)
    {
        var part = await _context.Parts
            .FirstOrDefaultAsync(p => p.PartId == partId, cancellationToken)
            ?? throw new NotFoundException(nameof(Part), partId);

        part.Name = request.Name;
        part.ModelNumber = request.ModelNumber;
        part.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);

        return ToResponse(part);
    }

    public async Task DeleteAsync(Guid partId, CancellationToken cancellationToken = default)
    {
        var part = await _context.Parts
            .FirstOrDefaultAsync(p => p.PartId == partId, cancellationToken)
            ?? throw new NotFoundException(nameof(Part), partId);

        _context.Parts.Remove(part);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static PartResponse ToResponse(Part p) => new(p.PartId, p.Name, p.ModelNumber, p.Description);
}
