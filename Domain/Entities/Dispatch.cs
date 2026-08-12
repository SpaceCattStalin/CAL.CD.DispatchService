namespace Domain;

public class Dispatch : BaseEntity
{
    public Guid DispatchId { get; init; }
    public Guid ShipperId { get; init; }
    public Guid CarrierId { get; private set; }
    public DispatchStatus DispatchStatus { get; private set; }
    public decimal Price { get; private set; }
    public DateTime PickupDate { get; private set; }
    public DateTime DropoffDate { get; private set; }
    public string? Description { get; private set; }
    public bool IsSigned { get; private set; }
    public Guid? PickupStopId { get; private set; }
    public Guid? DropoffStopId { get; private set; }
    public Stop? PickupStop { get; private set; }
    public Stop? DropoffStop { get; private set; }
    public ICollection<DispatchDriver> Drivers { get; private set; } = new List<DispatchDriver>();
    public ICollection<Vehicle> Vehicles { get; private set; } = new List<Vehicle>();

    /// <summary>
    /// Method to create isntance of Dispatch class
    /// </summary>
    /// <param name="shipperId">Id of Shipper company</param>
    /// <param name="carrierId">Id of Carrier company</param>
    /// <param name="price">Total price of the dispatch</param>
    /// <param name="pickupDate">Date when the dispatch is pickup</param>
    /// <param name="dropoffDate">Date when the dispatch is dropoff</param>
    /// <param name="description">Decription optional</param>
    /// <param name="pickupStop">Place where the dispatch is pickup</param>
    /// <param name="dropoffStop">Place where the dispatch is dropoff</param>
    /// <param name="vehicleInputs">A collection of object represent input to create a vehicle</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">The argument is empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">The argument exceeded the allowed range</exception>
    public static Dispatch Create(Guid shipperId, Guid carrierId, decimal price,
    DateTime pickupDate, DateTime dropoffDate, string? description,
    Stop pickupStop, Stop dropoffStop,
    IEnumerable<(string? Vin, int Year, string Make, string Model, string? Color)> vehicleInputs)
    {
        ArgumentNullException.ThrowIfNull(pickupStop);
        ArgumentNullException.ThrowIfNull(dropoffStop);
        ArgumentNullException.ThrowIfNull(vehicleInputs);

        if (shipperId == Guid.Empty)
            throw new ArgumentException("ShipperId is required", nameof(shipperId));

        if (carrierId == Guid.Empty)
            throw new ArgumentException("CarrierId is required", nameof(carrierId));

        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), price, "Price must be greater than 0");

        if (pickupDate < DateTime.UtcNow.Date)
            throw new ArgumentOutOfRangeException(nameof(pickupDate), pickupDate, "PickupDate cannot be in the past");

        // Check <= DateTime.UtcNow for case where DropOffDate is 30/5/2026 at 4PM and the input is 30/5/2026 at 5PM or 30/5/2026 at 3PM 
        if (dropoffDate <= DateTime.UtcNow || dropoffDate <= pickupDate)
            throw new ArgumentOutOfRangeException(nameof(dropoffDate), dropoffDate, "DropoffDate must be after both now and PickupDate");

        // Personal note: Use Count instead of Count()
        var vehicles = vehicleInputs.ToList();
        if (vehicles.Count < 1 || vehicles.Count > 12)
            throw new ArgumentOutOfRangeException(nameof(vehicleInputs), "A dispatch must have between 1 and 12 vehicles");

        var dispatchId = Guid.NewGuid();
        Dispatch dispatch = new()
        {
            DispatchId = dispatchId,
            ShipperId = shipperId,
            CarrierId = carrierId,
            Price = price,
            PickupDate = pickupDate,
            DropoffDate = dropoffDate,
            PickupStop = pickupStop,
            DropoffStop = dropoffStop,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var vehicleInput in vehicles)
        {
            var vehicle = Vehicle.CreateVehicle(dispatch, pickupStop, dropoffStop,
                vehicleInput.Vin, vehicleInput.Year, vehicleInput.Make, vehicleInput.Model, vehicleInput.Color);
            dispatch.Vehicles.Add(vehicle);
        }
        
        dispatch.UpdateStatus(DispatchStatus.NotSigned);

        return dispatch;
    }
    /// <summary>
    /// Update Dispatch stauts
    /// </summary>
    /// <param name="status">The Dispatch status to update</param>
    public void UpdateStatus(DispatchStatus status)
    {
        DispatchStatus = status;
        switch (status)
        {
            case DispatchStatus.PendingDelivery:
                foreach (var vehicle in Vehicles)
                    vehicle.UpdateStatus(VehicleStatus.PendingDelivery);
                break;

            case DispatchStatus.PendingPickup:
                foreach (var vehicle in Vehicles)
                    vehicle.UpdateStatus(VehicleStatus.PreparingForShipment);
                break;

            case DispatchStatus.Delivered:
                foreach (var vehicle in Vehicles)
                    vehicle.UpdateStatus(VehicleStatus.Delivered);
                break;

            case DispatchStatus.Canceled:
                foreach (var vehicle in Vehicles)
                    vehicle.UpdateStatus(VehicleStatus.Canceled);
                break;

            case DispatchStatus.NotSigned:
                foreach (var vehicle in Vehicles)
                    vehicle.UpdateStatus(VehicleStatus.NotSigned);
                break;
        }
    }
    public void Cancel()
    {
        UpdateStatus(DispatchStatus.Canceled);

        Vehicles.Clear();
        Drivers.Clear();
        PickupStop = null;
        PickupStopId = null;
        DropoffStop = null;
        DropoffStopId = null;
    }
}
