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
    public ICollection<DispatchDriver> Drivers { get; private set; }
    public ICollection<Vehicle> Vehicles { get; private set; }
}
