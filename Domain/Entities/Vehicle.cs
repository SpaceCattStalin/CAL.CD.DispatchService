namespace Domain;

public class Vehicle : BaseEntity
{
    public Guid VehicleId { get; init; }
    public Guid DispatchId { get; init; }
    public Guid PickupStopId { get; init; }
    public Guid DropoffStopId { get; init; }
    public VehicleStatus VehicleStatus { get; private set; }
    public string? Vin { get; init; }
    public int Year { get; init; }
    public string Make { get; init; }
    public string Model { get; init; }
    public string? Color { get; init; }
    public Dispatch Dispatch { get; init; }
    public Stop PickupStop { get; init; }
    public Stop DropoffStop { get; init; }

    /// <summary>
    /// Private method to create a Vehicle from VehicleInputs
    /// </summary>
    /// <param name="dispatch">The correlated Dispatch</param>
    /// <param name="pickupStop">Object of pickup Stop</param>
    /// <param name="dropoffStop">Object of the dropoff Stop</param>
    /// <param name="vin"></param>
    /// <param name="year"></param>
    /// <param name="make"></param>
    /// <param name="model"></param>
    /// <param name="color"></param>
    /// <returns>A Vehicle object</returns>
    public static Vehicle CreateVehicle(Dispatch dispatch, Stop pickupStop, Stop dropoffStop, string? vin,
    int year, string make, string model, string? color)
    {
        Vehicle vehicle = new()
        {
            VehicleId = Guid.NewGuid(),
            DispatchId = dispatch.DispatchId,
            Dispatch = dispatch,
            VehicleStatus = VehicleStatus.NotSigned,
            Vin = vin,
            Year = year,
            Make = make,
            Model = model,
            Color = color,
            PickupStop = pickupStop,
            DropoffStop = dropoffStop,
            PickupStopId = pickupStop.StopId,
            DropoffStopId = dropoffStop.StopId,
            CreatedAt = DateTime.UtcNow
        };

        return vehicle;
    }
}
