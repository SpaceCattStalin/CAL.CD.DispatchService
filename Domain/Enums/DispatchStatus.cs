using System.Text.Json.Serialization;

namespace Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DispatchStatus
{
    NotSigned,
    // Hien tren UI
    PendingPickup, // Dispatched
    PendingDelivery, // Picked Up
    Delivered,
    Canceled
}
