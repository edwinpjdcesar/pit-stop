using Core.Services.Interfaces;
using Data;
using Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class MakeService : IMakeService
{
    private readonly PitStopContext _context;

    public MakeService(PitStopContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<MakeResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Makes
            .OrderBy(m => m.Name)
            .Select(m => new MakeResponse(m.MakeId, m.Code, m.Name))
            .ToListAsync(cancellationToken);
    }
}
