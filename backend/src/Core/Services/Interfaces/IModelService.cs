using Domain.Dtos;

namespace Core.Services.Interfaces;

public interface IModelService
{
    Task<IReadOnlyList<ModelResponse>> GetByMakeAsync(int makeId, CancellationToken cancellationToken = default);
}
