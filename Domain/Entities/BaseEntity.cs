using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class BaseEntity
{
    // Postgres require the type to be "uint" to map to the native record version in Postgres
    // This property will not appear in the Postgres database, because it is mapped to a hidden "xmin" column in Postgres 
    // Reference: https://codewithmukesh.com/blog/concurrency-control-optimistic-locking-efcore/
    public uint RecordVersion { get; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }
}
