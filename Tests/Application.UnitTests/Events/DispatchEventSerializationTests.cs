using System.Text.Json;
using Application;
using Application.Events;
using Domain;

namespace Application.UnitTests.Events;

public class DispatchEventSerializationTests
{
    // private readonly Guid dispatchId = Guid.NewGuid();
    // private readonly Guid shipperId = Guid.NewGuid();
    // private readonly Guid carrierId = Guid.NewGuid();
    // private readonly DateTime createdAt = new DateTime(2026, 9, 24, 10, 20, 0, DateTimeKind.Utc);

    // [Fact]
    // public void DispatchWriterEvent_Serializes_WithRealFieldValues()
    // {
    //     var writerEvent = new DispatchWriterEvent(
    //         EventType.Create,
    //         dispatchId,
    //         shipperId,
    //         carrierId,
    //         1500m,
    //         DateTime.UtcNow,
    //         DateTime.UtcNow.AddDays(5),
    //         DispatchStatus.NotSigned,
    //         [new DispatchWriterVehicle("1HGCM82633A004352")],
    //         createdAt
    //         );

    //     var json = JsonSerializer.Serialize(writerEvent);

    //     Assert.NotEqual("{}", json);
    //     Assert.Contains(dispatchId.ToString(), json);
    //     Assert.Contains("1HGCM82633A004352", json);
    // }

    // [Fact]
    // public void DispatchUpdateEvent_Serializes_WithRealFieldValues()
    // {
    //     var updateEvent = new DispatchUpdateEvent(
    //         EventType.Update,
    //         dispatchId,
    //         shipperId,
    //         carrierId,
    //         1600m,
    //         DateTime.UtcNow,
    //         DateTime.UtcNow.AddDays(5),
    //         DispatchStatus.PendingPickup,
    //         [new DispatchUpdateVehicle("1HGCM82633A004352")],
    //         createdAt);

    //     var json = JsonSerializer.Serialize(updateEvent);

    //     Assert.NotEqual("{}", json);
    //     Assert.Contains(dispatchId.ToString(), json);
    //     Assert.Contains("1HGCM82633A004352", json);
    // }

    // [Fact]
    // public void DispatchDeleteEvent_Serializes_WithRealFieldValues()
    // {
    //     var dispatchId = Guid.NewGuid();
    //     var deleteEvent = new DispatchDeleteEvent(EventType.Delete, dispatchId);

    //     var json = JsonSerializer.Serialize(deleteEvent);

    //     Assert.NotEqual("{}", json);
    //     Assert.Contains(dispatchId.ToString(), json);
    // }
}
