using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class MakeService(PitStopContext context, ILogger<MakeService> logger) : IMakeService
{
    private readonly PitStopContext _context = context;
    private readonly ILogger<MakeService> _logger = logger;

    public async Task<IReadOnlyList<MakeResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all makes");

        var makes = await _context.Makes
            .OrderBy(m => m.Name)
            .Select(m => new MakeResponse(m.MakeId, m.Code, m.Name))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} makes", makes.Count);

        return makes;
    }
}
