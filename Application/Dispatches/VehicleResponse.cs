namespace Application.Dispatches;

public record VehicleResponse(
    Guid VehicleId,
    string VehicleStatus,
    string? Vin,
    int Year,
    string Make,
    string Model,
    string? Color,
    StopResponse PickupStop,
    StopResponse DropoffStop);
