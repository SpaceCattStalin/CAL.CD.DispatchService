namespace Domain;

public class Vehicle : BaseEntity
{
    public Guid VehicleId { get; init; }
    public Guid DispatchId { get; init; }
    public Guid PickupStopId { get; private set; }
    public Guid DropoffStopId { get; private set; }
    public VehicleStatus VehicleStatus { get; private set; }
    public string Vin { get; private set; }
    public short Year { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public string Color { get; private set; }
    public Dispatch Dispatch { get; private set; }
    public Stop PickupStop { get; private set; }
    public Stop DropoffStop { get; private set; }
}
