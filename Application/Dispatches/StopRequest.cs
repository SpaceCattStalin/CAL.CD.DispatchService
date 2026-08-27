namespace Application.Dispatches;

public class StopRequest(
    string Address,
    string? LocationName,
    string? ContactName,
    string? ContactPhone,
    string? ContactEmail)
{
    public string Address { get; init; } = Address;
    public string? LocationName { get; init; } = LocationName;
    public string? ContactName { get; init; } = ContactName;
    public string? ContactPhone { get; init; } = ContactPhone;
    public string? ContactEmail { get; init; } = ContactEmail;
}
