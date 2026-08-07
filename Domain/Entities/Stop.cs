namespace Domain;

public class Stop : BaseEntity
{
    public Guid StopId { get; init; }
    public StopNumber StopNumber { get; private set; }
    public string Address { get; set; }
    public string? LocationName { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
}
