namespace Domain;

public class DispatchDriver : BaseEntity
{
    public Guid DispatchId { get; init; }
    public Guid DriverId { get; init; }
    public User Driver { get; init; }
    public Dispatch Dispatch { get; init; }
}
