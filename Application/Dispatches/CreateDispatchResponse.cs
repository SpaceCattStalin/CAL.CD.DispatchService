namespace Application.Dispatches;

public record CreateDispatchResponse(
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
    DateTime CreatedAt);
