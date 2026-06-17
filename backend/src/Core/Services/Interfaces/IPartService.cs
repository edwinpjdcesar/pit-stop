using Domain.Dtos;

namespace Core.Services.Interfaces;

public interface IPartService
{
    Task<IReadOnlyList<PartResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PartResponse?> GetByIdAsync(Guid partId, CancellationToken cancellationToken = default);
    Task<PartResponse> CreateAsync(PartRequest request, CancellationToken cancellationToken = default);
    Task<PartResponse> UpdateAsync(Guid partId, PartRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid partId, CancellationToken cancellationToken = default);
}
