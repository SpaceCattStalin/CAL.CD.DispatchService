using Domain;

namespace Application;

public class DispatchUpdateEvent(
    EventType Type,
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchUpdateVehicle> Vehicles);
public class DispatchUpdateVehicle(string? Vin);

