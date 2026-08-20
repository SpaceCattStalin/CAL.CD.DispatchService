using System.Text.Json.Serialization;

namespace Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DispatchStatus
{
    NotSigned,
    PendingPickup,
    PendingDelivery,
    Delivered,
    Canceled
}
