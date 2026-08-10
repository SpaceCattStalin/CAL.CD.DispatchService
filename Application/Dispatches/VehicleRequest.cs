namespace Application.Dispatches;

public record VehicleRequest(
    string? Vin,
    int Year,
    string Make,
    string Model,
    string? Color);
