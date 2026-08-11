namespace Application.Dispatches;

public record StopResponse(
    Guid StopId,
    string StopNumber,
    string Address,
    string? LocationName,
    string? ContactName,
    string? ContactPhone,
    string? ContactEmail);
