namespace Domain;

public class Dispatch : BaseEntity
{
    public Guid DispatchId { get; init; }
    public Guid ShipperId { get; init; }
    public Guid CarrierId { get; private set; }
    public DispatchStatus DispatchStatus { get; private set; }
    public decimal PriceTotal { get; private set; }
    public string Instructions { get; private set; }
    public ICollection<Vehicle> Vehicles { get; private set; }
}
