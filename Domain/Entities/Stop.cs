namespace Domain;

public class Stop : BaseEntity
{
    public Guid StopId { get; init; }
    public int StopNumber { get; private set; }
    public string Address { get; private set; }
    public string? LocationName { get; private set; }
    public string? ContactName { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? ContactEmail { get; private set; }

    /// <summary>
    /// Factory method to create an instance of Stop
    /// </summary>
    /// <param name="stopNumber">Whether this is the Pickup or Dropoff stop</param>
    /// <param name="address">Physical address of the stop</param>
    /// <param name="locationName">Optional name of the location</param>
    /// <param name="contactName">Optional contact name at the stop</param>
    /// <param name="contactPhone">Optional contact phone at the stop</param>
    /// <param name="contactEmail">Optional contact email at the stop</param>
    /// <returns>A Stop object</returns>
    /// <exception cref="ArgumentException">Address is missing</exception>
    /// <exception cref="ArgumentOutOfRangeException">StopNumber is not 1 or 2</exception>
    public static Stop Create(int stopNumber, string address, string? locationName,
        string? contactName, string? contactPhone, string? contactEmail)
    {
        if (stopNumber != 1 && stopNumber != 2)
            throw new ArgumentOutOfRangeException(nameof(stopNumber), stopNumber, "StopNumber must be 1 (pickup) or 2 (dropoff)");
            
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required", nameof(address));

        return new Stop
        {
            StopId = Guid.NewGuid(),
            StopNumber = stopNumber,
            Address = address,
            LocationName = locationName,
            ContactName = contactName,
            ContactPhone = contactPhone,
            ContactEmail = contactEmail,
            CreatedAt = DateTime.UtcNow
        };
    }
}
