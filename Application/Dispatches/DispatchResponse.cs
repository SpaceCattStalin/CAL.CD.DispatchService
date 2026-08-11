namespace Application.Dispatches;

public record DispatchResponse(
    Guid DispatchId,
    Guid ShipperId,
    Guid CarrierId,
    string DispatchStatus,
    decimal Price,
    DateTime PickupDate,
    DateTime DropoffDate,
    string? Description,
    bool IsSigned,
    StopResponse PickupStop,
    StopResponse DropoffStop,
    IEnumerable<VehicleResponse> Vehicles,
    IEnumerable<DriverResponse> Drivers,
    DateTime CreatedAt);
