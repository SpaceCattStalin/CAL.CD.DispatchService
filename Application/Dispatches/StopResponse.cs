namespace Application.Dispatches;

public class StopResponse(
    Guid StopId,
    string StopNumber,
    string Address,
    string? LocationName,
    string? ContactName,
    string? ContactPhone,
    string? ContactEmail)
{
    public Guid StopId { get; init; } = StopId;
    public string StopNumber { get; init; } = StopNumber;
    public string Address { get; init; } = Address;
    public string? LocationName { get; init; } = LocationName;
    public string? ContactName { get; init; } = ContactName;
    public string? ContactPhone { get; init; } = ContactPhone;
    public string? ContactEmail { get; init; } = ContactEmail;
}
