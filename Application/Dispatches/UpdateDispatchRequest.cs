namespace Application.Dispatches;

public class UpdateDispatchRequest(
    decimal Price,
    DateTime PickupDate,
    DateTime DropoffDate,
    string? Description,
    StopRequest PickupStop,
    StopRequest DropoffStop,
    IEnumerable<UpdateVehicleRequest> Vehicles)
{
    public decimal Price { get; init; } = Price;
    public DateTime PickupDate { get; init; } = PickupDate;
    public DateTime DropoffDate { get; init; } = DropoffDate;
    public string? Description { get; init; } = Description;
    public StopRequest PickupStop { get; init; } = PickupStop;
    public StopRequest DropoffStop { get; init; } = DropoffStop;
    public IEnumerable<UpdateVehicleRequest> Vehicles { get; init; } = Vehicles;
}
