using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class ModelService : IModelService
{
    private readonly PitStopContext _context;

    public ModelService(PitStopContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ModelResponse>> GetByMakeAsync(int makeId, CancellationToken cancellationToken = default)
    {
        var makeExists = await _context.Makes.AnyAsync(m => m.MakeId == makeId, cancellationToken);
        if (!makeExists)
            throw new NotFoundException(nameof(Make), makeId);

        return await _context.Models
            .Where(m => m.MakeId == makeId)
            .OrderBy(m => m.Name)
            .Select(m => new ModelResponse(m.ModelId, m.MakeId, m.Make.Name, m.Code, m.Name))
            .ToListAsync(cancellationToken);
    }
}
