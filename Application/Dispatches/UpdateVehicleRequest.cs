namespace Application.Dispatches;

public record UpdateVehicleRequest(
    Guid? VehicleId,
    string? Vin,
    int? Year,
    string? Make,
    string? Model,
    string? Color);
