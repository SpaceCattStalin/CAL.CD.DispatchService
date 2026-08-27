namespace Application.Dispatches;

public class VehicleResponse(
    Guid VehicleId,
    string VehicleStatus,
    string? Vin,
    int Year,
    string Make,
    string Model,
    string? Color,
    StopResponse PickupStop,
    StopResponse DropoffStop)
{
    public Guid VehicleId { get; init; } = VehicleId;
    public string VehicleStatus { get; init; } = VehicleStatus;
    public string? Vin { get; init; } = Vin;
    public int Year { get; init; } = Year;
    public string Make { get; init; } = Make;
    public string Model { get; init; } = Model;
    public string? Color { get; init; } = Color;
    public StopResponse PickupStop { get; init; } = PickupStop;
    public StopResponse DropoffStop { get; init; } = DropoffStop;
}
