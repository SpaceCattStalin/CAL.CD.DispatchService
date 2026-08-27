using Application.Events;
using Domain;

namespace Application.Dispatches;

// Mirrors Application.Events.DispatchWriterEvent minus the EventType field —
// a paginated GET response has no Create/Update/Delete concept, only current state.
public class DispatchWriterDto(
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchWriterVehicle> Vehicles)
{
    public Guid DispatchId { get; init; } = DispatchId;
    public decimal PriceTotal { get; init; } = PriceTotal;
    public DateTime PickupDate { get; init; } = PickupDate;
    public DateTime DropoffDate { get; init; } = DropoffDate;
    public DispatchStatus DispatchStatus { get; init; } = DispatchStatus;
    public IEnumerable<DispatchWriterVehicle> Vehicles { get; init; } = Vehicles;
}
