namespace Application.Dispatches;

public class DriverResponse(
    Guid DriverId,
    string FirstName,
    string LastName,
    string Phone,
    string Email)
{
    public Guid DriverId { get; init; } = DriverId;
    public string FirstName { get; init; } = FirstName;
    public string LastName { get; init; } = LastName;
    public string Phone { get; init; } = Phone;
    public string Email { get; init; } = Email;
}
