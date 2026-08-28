using Application.Events;
using Domain;

namespace Application.Dispatches;

// Mirrors Application.Events.DispatchWriterEvent minus the EventType field —
// a paginated GET response has no Create/Update/Delete concept, only current state.
public record DispatchWriterDto(
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchWriterVehicle> Vehicles);
