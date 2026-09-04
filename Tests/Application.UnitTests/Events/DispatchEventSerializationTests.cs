using System.Text.Json;
using Application;
using Application.Events;
using Domain;

namespace Application.UnitTests.Events;

// Regression coverage for a bug where DispatchWriterEvent/DispatchUpdateEvent/DispatchDeleteEvent
// were plain classes using primary-constructor syntax with no explicit property declarations.
// That compiles fine, but System.Text.Json has nothing to see: JsonSerializer.Serialize produced
// "{}" for every published event, which meant every downstream consumer (SearchJobs) deserialized
// a completely empty DispatchWriterEvent and crashed on its null Vehicles collection.
public class DispatchEventSerializationTests
{
    [Fact]
    public void DispatchWriterEvent_Serializes_WithRealFieldValues()
    {
        var dispatchId = Guid.NewGuid();
        var writerEvent = new DispatchWriterEvent(
            EventType.Create,
            dispatchId,
            1500m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(5),
            DispatchStatus.NotSigned,
            [new DispatchWriterVehicle("1HGCM82633A004352")]);

        var json = JsonSerializer.Serialize(writerEvent);

        Assert.NotEqual("{}", json);
        Assert.Contains(dispatchId.ToString(), json);
        Assert.Contains("1HGCM82633A004352", json);
    }

    [Fact]
    public void DispatchUpdateEvent_Serializes_WithRealFieldValues()
    {
        var dispatchId = Guid.NewGuid();
        var updateEvent = new DispatchUpdateEvent(
            EventType.Update,
            dispatchId,
            1600m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(5),
            DispatchStatus.PendingPickup,
            [new DispatchUpdateVehicle("1HGCM82633A004352")]);

        var json = JsonSerializer.Serialize(updateEvent);

        Assert.NotEqual("{}", json);
        Assert.Contains(dispatchId.ToString(), json);
        Assert.Contains("1HGCM82633A004352", json);
    }

    [Fact]
    public void DispatchDeleteEvent_Serializes_WithRealFieldValues()
    {
        var dispatchId = Guid.NewGuid();
        var deleteEvent = new DispatchDeleteEvent(EventType.Delete, dispatchId);

        var json = JsonSerializer.Serialize(deleteEvent);

        Assert.NotEqual("{}", json);
        Assert.Contains(dispatchId.ToString(), json);
    }
}
