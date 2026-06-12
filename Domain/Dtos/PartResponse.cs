namespace Domain.Dtos;

public record PartResponse(
    Guid PartId,
    string Name,
    string? ModelNumber,
    string Description
);
