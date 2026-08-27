using Domain;

namespace Application.Events;

public class DispatchWriterEvent(
    EventType Type,
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchWriterVehicle> Vehicles);

public class DispatchWriterVehicle(string? Vin);
