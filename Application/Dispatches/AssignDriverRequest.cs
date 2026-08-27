namespace Application.Dispatches;

public class AssignDriverRequest(
    Guid DriverId)
{
    public Guid DriverId { get; init; } = DriverId;
}
