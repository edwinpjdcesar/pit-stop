using Domain.Dtos;

namespace Core.Services.Interfaces;

public interface IMakeService
{
    Task<IReadOnlyList<MakeResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
