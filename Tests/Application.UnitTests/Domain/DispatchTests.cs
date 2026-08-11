using Application.Dispatches;
using Domain;

namespace Application.UnitTests.Domain;

public class DispatchTests
{
    private static readonly Guid shipperId = Guid.NewGuid();
    private static readonly Guid carrierId = Guid.NewGuid();
    private const decimal price = 100m;
    private static readonly DateTime pickupDate = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime dropoffDate = DateTime.UtcNow.AddDays(2);
    private const string description = "Test dispatch";
    private static readonly Stop pickupStop = Stop.Create(
        1,
        "123 Main Street, Xo Viet Nghe Tinh",
        "Phuong 27",
        "John Overwatch",
        "555-123-4567",
        "john@example.com");
    private static readonly Stop dropoffStop = Stop.Create(
        2,
        "123 Main Street, Le Van Viet",
        "Phuong 27",
        "John Valorant",
        "555-123-4589",
        "johnny@example.com");
    private static readonly (string? Vin, int Year, string Make, string Model, string? Color) defaultVehicle =
        ("1HGCM82633A1", 2020, "Honda", "Accord", "Blue");

    private static (string? Vin, int Year, string Make, string Model, string? Color)[] MakeVehicleInputs(int count) =>
        Enumerable.Range(0, count)
        .Select(x => ("1HGC13232141", 2020, "Honda", "Accord", "Blue"))
        .ToArray();

    [Fact]
    public void Create_ValidInputs_ReturnsDispatchWithDefaults()
    {
        var dispatch = Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, new[] { defaultVehicle });

        Assert.Equal(DispatchStatus.NotSigned, dispatch.DispatchStatus);
        Assert.NotEqual(Guid.Empty, dispatch.DispatchId);
    }

    [Fact]
    public void Create_NullPickupStop_ThrowsArgumentNullException()
    {
        Action action = () => Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop: null!, dropoffStop, new[] { defaultVehicle });
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void Create_NullDropoffStop_ThrowsArgumentNullException()
    {
        Action action = () => Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop: null!, new[] { defaultVehicle });
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void Create_NullVehicleInputs_ThrowsArgumentNullException()
    {
        Action action = () => Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, vehicleInputs: null!);
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void Create_EmptyShipperId_ThrowsArgumentException()
    {
        Action action = () => Dispatch.Create(Guid.Empty, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, new[] { defaultVehicle });
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_EmptyCarrierId_ThrowsArgumentException()
    {
        Action action = () => Dispatch.Create(shipperId, Guid.Empty, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, new[] { defaultVehicle });
        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_NonPositivePrice_ThrowsArgumentOutOfRangeException(decimal price)
    {
        Action action = () => Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, new[] { defaultVehicle });
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Create_PastPickupDate_ThrowsArgumentOutOfRangeException()
    {
        Action action = () => Dispatch.Create(shipperId, carrierId, price,
            pickupDate: DateTime.UtcNow.AddDays(-1), dropoffDate, description,
            pickupStop, dropoffStop, new[] { defaultVehicle });
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Create_DropoffDateNotAfterNow_ThrowsArgumentOutOfRangeException()
    {
        var pickup = DateTime.UtcNow.AddDays(1);
        Action action = () => Dispatch.Create(shipperId, carrierId, price, pickup,
            dropoffDate: DateTime.UtcNow, description, pickupStop, dropoffStop, new[] { defaultVehicle });
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Create_DropoffDateNotAfterPickupDate_ThrowsArgumentOutOfRangeException()
    {
        var pickup = DateTime.UtcNow.AddDays(2);
        Action action = () => Dispatch.Create(shipperId, carrierId, price, pickup,
            dropoffDate: pickup, description, pickupStop, dropoffStop, new[] { defaultVehicle });
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Create_VehicleCountOutOfRange_ThrowsArgumentOutOfRangeException(int count)
    {
        Action action = () => Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, MakeVehicleInputs(count));
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Create_ValidInputs_VehicleCountMatchesInput()
    {
        var inputs = MakeVehicleInputs(count: 5);
        var dispatch = Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, inputs);
        Assert.Equal(5, dispatch.Vehicles.Count);
    }

    [Fact]
    public void Create_ValidInputs_AllVehiclesShareDispatchStops()
    {
        var pickup = Stop.Create(1, "123 Main St", null, null, null, null);
        var dropoff = Stop.Create(2, "456 Oak Ave", null, null, null, null);
        var dispatch = Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickup, dropoff, MakeVehicleInputs(count: 3));

        Assert.All(dispatch.Vehicles, vehicle =>
        {
            Assert.Equal(dispatch.PickupStop.StopId, vehicle.PickupStopId);
            Assert.Equal(dispatch.DropoffStop.StopId, vehicle.DropoffStopId);
            Assert.Same(dispatch.PickupStop, vehicle.PickupStop);
            Assert.Same(dispatch.DropoffStop, vehicle.DropoffStop);
        });
    }
}
