namespace Domain.Dtos;

public record PartRequest(
    string Name,
    string? ModelNumber,
    string Description
);
