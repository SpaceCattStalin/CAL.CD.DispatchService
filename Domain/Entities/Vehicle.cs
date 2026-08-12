namespace Domain;

public class Vehicle : BaseEntity
{
    public Guid VehicleId { get; init; }
    public Guid DispatchId { get; init; }
    public Guid PickupStopId { get; init; }
    public Guid DropoffStopId { get; init; }
    public VehicleStatus VehicleStatus { get; private set; }
    public string? Vin { get; private set; }
    public int Year { get; init; }
    public string Make { get; init; }
    public string Model { get; init; }
    public string? Color { get; private set; }
    public Dispatch Dispatch { get; init; }
    public Stop PickupStop { get; init; }
    public Stop DropoffStop { get; init; }

    /// <summary>
    /// Factory method to create a Vehicle associated with a Dispatch.
    /// </summary>
    /// <param name="dispatch">The Dispatch this vehicle belongs to</param>
    /// <param name="pickupStop">Stop where the vehicle is picked up</param>
    /// <param name="dropoffStop">Stop where the vehicle is dropped off</param>
    /// <param name="vin">Vehicle identification number, optional</param>
    /// <param name="year">Model year of the vehicle</param>
    /// <param name="make">Manufacturer of the vehicle</param>
    /// <param name="model">Model name of the vehicle</param>
    /// <param name="color">Color of the vehicle, optional</param>
    /// <returns>A new Vehicle instance with status NotSigned</returns>
    /// <exception cref="ArgumentNullException">dispatch, pickupStop, or dropoffStop is null</exception>
    /// <exception cref="ArgumentException">make or model is empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">year is outside the valid range</exception>
    public static Vehicle CreateVehicle(Dispatch dispatch, Stop pickupStop, Stop dropoffStop, string? vin,
    int year, string make, string model, string? color)
    {
        ArgumentNullException.ThrowIfNull(dispatch);
        ArgumentNullException.ThrowIfNull(pickupStop);
        ArgumentNullException.ThrowIfNull(dropoffStop);

        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("Make is required", nameof(make));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model is required", nameof(model));

        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentOutOfRangeException(nameof(year), year, "Year must be between 1900 and next year");

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
    /// <summary>
    /// Update a Vehicle status
    /// </summary>
    /// <param name="status">Vehicle status to update</param>
    public void UpdateStatus(VehicleStatus status)
    {
        VehicleStatus = status;
    }

    /// <summary>
    /// Update this Vehicle's editable fields. Vin and Color are the only editable fields;
    /// Year, Make, and Model are fixed at creation.
    /// </summary>
    public void UpdateDetails(string? vin, string? color)
    {
        Vin = vin;
        Color = color;
    }
}
