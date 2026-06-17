namespace Domain.Dtos;

public record ModelResponse(
    int ModelId,
    int MakeId,
    string MakeName,
    string Code,
    string Name
);
