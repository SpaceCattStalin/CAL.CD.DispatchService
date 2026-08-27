namespace Application.Dispatches;

public class DispatchResponse(
    Guid DispatchId,
    Guid ShipperId,
    Guid CarrierId,
    string DispatchStatus,
    decimal Price,
    DateTime PickupDate,
    DateTime DropoffDate,
    string? Description,
    bool IsSigned,
    StopResponse? PickupStop,
    StopResponse? DropoffStop,
    IEnumerable<VehicleResponse> Vehicles,
    IEnumerable<DriverResponse> Drivers,
    DateTime CreatedAt)
{
    public Guid DispatchId { get; init; } = DispatchId;
    public Guid ShipperId { get; init; } = ShipperId;
    public Guid CarrierId { get; init; } = CarrierId;
    public string DispatchStatus { get; init; } = DispatchStatus;
    public decimal Price { get; init; } = Price;
    public DateTime PickupDate { get; init; } = PickupDate;
    public DateTime DropoffDate { get; init; } = DropoffDate;
    public string? Description { get; init; } = Description;
    public bool IsSigned { get; init; } = IsSigned;
    public StopResponse? PickupStop { get; init; } = PickupStop;
    public StopResponse? DropoffStop { get; init; } = DropoffStop;
    public IEnumerable<VehicleResponse> Vehicles { get; init; } = Vehicles;
    public IEnumerable<DriverResponse> Drivers { get; init; } = Drivers;
    public DateTime CreatedAt { get; init; } = CreatedAt;
}
