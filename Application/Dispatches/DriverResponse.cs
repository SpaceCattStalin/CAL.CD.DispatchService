namespace Application.Dispatches;

public record DriverResponse(
    Guid DriverId,
    string FirstName,
    string LastName,
    string Phone,
    string Email);
