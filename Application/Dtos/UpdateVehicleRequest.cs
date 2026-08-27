namespace Application.Dispatches;

public class UpdateVehicleRequest(
    Guid VehicleId,
    string? Vin,
    int? Year,
    string? Make,
    string? Model,
    string? Color)
{
    public Guid VehicleId { get; init; } = VehicleId;
    public string? Vin { get; init; } = Vin;
    public int? Year { get; init; } = Year;
    public string? Make { get; init; } = Make;
    public string? Model { get; init; } = Model;
    public string? Color { get; init; } = Color;
}
