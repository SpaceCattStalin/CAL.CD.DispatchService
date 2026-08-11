namespace Application.Dispatches;

public record StopRequest(
    string Address,
    string? LocationName,
    string? ContactName,
    string? ContactPhone,
    string? ContactEmail);
