using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class ModelService(PitStopContext context, ILogger<ModelService> logger) : IModelService
{
    private readonly PitStopContext _context = context;
    private readonly ILogger<ModelService> _logger = logger;

    public async Task<IReadOnlyList<ModelResponse>> GetByMakeAsync(int makeId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting models for make {MakeId}", makeId);

        var makeExists = await _context.Makes.AnyAsync(m => m.MakeId == makeId, cancellationToken);
        if (!makeExists)
        {
            _logger.LogWarning("Make {MakeId} not found", makeId);
            throw new NotFoundException(nameof(Make), makeId);
        }

        var models = await _context.Models
            .Where(m => m.MakeId == makeId)
            .OrderBy(m => m.Name)
            .Select(m => new ModelResponse(m.ModelId, m.MakeId, m.Make.Name, m.Code, m.Name))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} models for make {MakeId}", models.Count, makeId);

        return models;
    }
}
