namespace Domain;

public class DispatchDriver
{
    public Guid DispatchId { get; init; }
    public Guid DriverId { get; init; }
    public User Driver { get; init; }
    public Dispatch Dispatch { get; init; }
}
