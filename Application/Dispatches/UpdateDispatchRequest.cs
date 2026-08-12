namespace Application.Dispatches;

public record UpdateDispatchRequest(
    decimal Price,
    DateTime PickupDate,
    DateTime DropoffDate,
    string? Description,
    StopRequest PickupStop,
    StopRequest DropoffStop,
    IEnumerable<UpdateVehicleRequest> Vehicles);
