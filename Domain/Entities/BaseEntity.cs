namespace Domain;

public class BaseEntity
{
    // TO-DO: remember to config the EF Core configuration to mark this property as a RowVersion
    public byte[] RecordVersion { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }
    /// <summary>
    /// Update the UpdateAt property when this method is called
    /// </summary>
    protected void Touch() => UpdatedAt = DateTime.UtcNow;    
}
