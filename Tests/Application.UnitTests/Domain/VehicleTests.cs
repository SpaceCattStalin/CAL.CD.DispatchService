using Domain;

namespace Application.UnitTests.Domain;

public class VehicleTests
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
    private readonly Dispatch dispatch = Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, new[] { defaultVehicle });

    [Fact]
    public void CreateVehicle_ValidInputs_ReturnsVehicleWithDefaults()
    {
        var vehicle = Vehicle.CreateVehicle(dispatch, pickupStop, dropoffStop, "1HGCM82633A00", 2020, "Honda", "Civic", "Bule");

        Assert.NotEqual(Guid.Empty, vehicle.VehicleId);

        Assert.Equal(VehicleStatus.NotSigned, vehicle.VehicleStatus);
        Assert.Equal(dispatch.DispatchId, vehicle.DispatchId);
        Assert.Equal(dispatch.PickupStop, vehicle.PickupStop);
        Assert.Equal(dispatch.DropoffStop, vehicle.DropoffStop);
    }

    [Fact]
    public void CreateVehicle_NullVin_DoesNotThrowsArgumentException()
    {
        var vehicle = Vehicle.CreateVehicle(dispatch, pickupStop, dropoffStop,
            null, defaultVehicle.Year, defaultVehicle.Make, defaultVehicle.Model, defaultVehicle.Color);

        Assert.Null(vehicle.Vin);
    }

    [Fact]
    public void CreateVehicle_NullColor_DoesNotThrowsArgumentException()
    {
        var vehicle = Vehicle.CreateVehicle(dispatch, pickupStop, dropoffStop,
            defaultVehicle.Vin, defaultVehicle.Year, defaultVehicle.Make, defaultVehicle.Model, null);

        Assert.Null(vehicle.Color);
    }

    [Fact]
    public void CreateVehicle_NullDispatch_ThrowsArgumentException()
    {
        Action action = () => Vehicle.CreateVehicle(null, pickupStop, dropoffStop,
            defaultVehicle.Vin, defaultVehicle.Year, defaultVehicle.Make, defaultVehicle.Model, defaultVehicle.Color);

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void CreateVehicle_NullPickupStop_ThrowsArgumentException()
    {
        Action action = () => Vehicle.CreateVehicle(dispatch, null, dropoffStop,
            defaultVehicle.Vin, defaultVehicle.Year, defaultVehicle.Make, defaultVehicle.Model, defaultVehicle.Color);

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void CreateVehicle_NullDropoffStop_ThrowsArgumentException()
    {
        Action action = () => Vehicle.CreateVehicle(dispatch, pickupStop, null,
            defaultVehicle.Vin, defaultVehicle.Year, defaultVehicle.Make, defaultVehicle.Model, defaultVehicle.Color);

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void CreateVehicle_EmptyMake_ThrowsArgumentException()
    {
        Action action = () => Vehicle.CreateVehicle(dispatch, pickupStop, dropoffStop,
            defaultVehicle.Vin, defaultVehicle.Year, "", defaultVehicle.Model, defaultVehicle.Color);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void CreateVehicle_EmptyModel_ThrowsArgumentException()
    {
        Action action = () => Vehicle.CreateVehicle(dispatch, pickupStop, dropoffStop,
            defaultVehicle.Vin, defaultVehicle.Year, defaultVehicle.Make, "", defaultVehicle.Color);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void CreateVehicle_YearTooLow_ThrowsArgumentException()
    {
        Action action = () => Vehicle.CreateVehicle(dispatch, pickupStop, dropoffStop,
            defaultVehicle.Vin, 1899, defaultVehicle.Make, defaultVehicle.Model, defaultVehicle.Color);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void CreateVehicle_YearTooHigh_ThrowsArgumentException()
    {
        Action action = () => Vehicle.CreateVehicle(dispatch, pickupStop, dropoffStop,
            defaultVehicle.Vin, DateTime.UtcNow.Year + 2, defaultVehicle.Make, defaultVehicle.Model, defaultVehicle.Color);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}
