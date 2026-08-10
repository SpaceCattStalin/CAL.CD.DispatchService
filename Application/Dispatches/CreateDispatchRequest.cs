namespace Application.Dispatches;

public record CreateDispatchRequest(
    Guid CarrierId,
    decimal Price,
    DateTime PickupDate,
    DateTime DropoffDate,
    string? Description,
    StopRequest PickupStop,
    StopRequest DropoffStop,
    IEnumerable<VehicleRequest> Vehicles);
