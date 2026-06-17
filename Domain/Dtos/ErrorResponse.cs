namespace Domain.Dtos;

public record ErrorDetail(string Code, string Message, IReadOnlyList<string>? Details = null);

public record ErrorResponse(ErrorDetail Error);
