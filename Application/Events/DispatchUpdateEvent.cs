using Domain;

namespace Application;

public record class DispatchUpdateEvent(
    EventType Type,
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchUpdateVehicle> Vehicles);
public record class DispatchUpdateVehicle(string? Vin);

