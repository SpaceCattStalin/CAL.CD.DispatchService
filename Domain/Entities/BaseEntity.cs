namespace Domain;

public class BaseEntity
{
    // TO-DO: remember to config the EF Core configuration to mark this property as a RowVersion
    public byte[] RecordVersion { get; private set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; private set; }
    /// <summary>
    /// Update the UpdateAt property when this method is called
    /// </summary>
    protected void Touch() => UpdatedAt = DateTimeOffset.UtcNow;    
}
