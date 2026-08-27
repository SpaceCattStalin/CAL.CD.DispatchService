using Application.Dispatches;
using Domain;

namespace Application.UnitTests.Dispatches;

public class DispatchMapperTests
{
    private static readonly Guid shipperId = Guid.NewGuid();
    private static readonly Guid carrierId = Guid.NewGuid();
    private const decimal price = 100m;
    private static readonly DateTime pickupDate = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime dropoffDate = DateTime.UtcNow.AddDays(2);
    private const string description = "Test dispatch";

    private static readonly StopRequest pickupStopRequest = new(
        "123 Main Street, Xo Viet Nghe Tinh",
        "Phuong So 27",
        "John Overwatch",
        "555-123-4567",
        "john@example.com");
    private static readonly StopRequest dropoffStopRequest = new(
        "123 Main Street, Le Van Viet",
        "Phuong So 27",
        "John Valorant",
        "555-123-4589",
        "johnny@example.com");
    private static readonly VehicleRequest defaultVehicleRequest = new(
        "1HGCM82633A1", 2020, "Honda", "Accord", "Blue");

    private static CreateDispatchRequest MakeRequest(IEnumerable<VehicleRequest> vehicles) =>
        new(carrierId, price, pickupDate, dropoffDate, description,
            pickupStopRequest, dropoffStopRequest, vehicles);

    private static readonly Stop pickupStop = Stop.Create(1,
        pickupStopRequest.Address, pickupStopRequest.LocationName,
        pickupStopRequest.ContactName, pickupStopRequest.ContactPhone, pickupStopRequest.ContactEmail);
    private static readonly Stop dropoffStop = Stop.Create(2,
        dropoffStopRequest.Address, dropoffStopRequest.LocationName,
        dropoffStopRequest.ContactName, dropoffStopRequest.ContactPhone, dropoffStopRequest.ContactEmail);
    private static readonly (string? Vin, int Year, string Make, string Model, string? Color) defaultVehicle =
        ("1HGCM82633A1", 2020, "Honda", "Accord", "Blue");

    [Fact]
    public void ToDomain_ValidRequest_ReturnsDispatchWithMappedFields()
    {
        var request = MakeRequest(new[] { defaultVehicleRequest });

        var dispatch = DispatchMapper.ToDomain(request, shipperId);

        Assert.Equal(shipperId, dispatch.ShipperId);
        Assert.Equal(request.CarrierId, dispatch.CarrierId);
        Assert.Equal(request.Price, dispatch.Price);
        Assert.Equal(request.PickupDate, dispatch.PickupDate);
        Assert.Equal(request.DropoffDate, dispatch.DropoffDate);
        Assert.Equal(request.Description, dispatch.Description);
        Assert.Equal(1, dispatch.PickupStop!.StopNumber);
        Assert.Equal(request.PickupStop.Address, dispatch.PickupStop.Address);
        Assert.Equal(2, dispatch.DropoffStop!.StopNumber);
        Assert.Equal(request.DropoffStop.Address, dispatch.DropoffStop.Address);
    }

    [Fact]
    public void ToDomain_MultipleVehicles_MapsAllVehiclesWithMatchingFields()
    {
        var vehicleRequests = new[]
        {
            new VehicleRequest("1HGCM82633A1", 2020, "Honda", "Accord", "Blue"),
            new VehicleRequest("2HGCM82633A2", 2021, "Toyota", "Corolla", "Red"),
            new VehicleRequest(null, 2022, "Ford", "Focus", null),
        };
        var request = MakeRequest(vehicleRequests);

        var dispatch = DispatchMapper.ToDomain(request, shipperId);

        Assert.Equal(3, dispatch.Vehicles.Count);
        var vehicles = dispatch.Vehicles.ToList();
        for (int i = 0; i < vehicleRequests.Length; i++)
        {
            Assert.Equal(vehicleRequests[i].Vin, vehicles[i].Vin);
            Assert.Equal(vehicleRequests[i].Year, vehicles[i].Year);
            Assert.Equal(vehicleRequests[i].Make, vehicles[i].Make);
            Assert.Equal(vehicleRequests[i].Model, vehicles[i].Model);
            Assert.Equal(vehicleRequests[i].Color, vehicles[i].Color);
        }
    }

    [Fact]
    public void ToDomain_InvalidPickupStopAddress_ThrowsArgumentException()
    {
        var invalidPickupStop = new StopRequest("", pickupStopRequest.LocationName, pickupStopRequest.ContactName, pickupStopRequest.ContactPhone, pickupStopRequest.ContactEmail);
        var baseRequest = MakeRequest(new[] { defaultVehicleRequest });
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, baseRequest.PickupDate, baseRequest.DropoffDate, baseRequest.Description, invalidPickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);

        Action action = () => DispatchMapper.ToDomain(request, shipperId);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void ToDomain_InvalidPrice_ThrowsArgumentOutOfRangeException()
    {
        var baseRequest = MakeRequest(new[] { defaultVehicleRequest });
        var request = new CreateDispatchRequest(baseRequest.CarrierId, 0, baseRequest.PickupDate, baseRequest.DropoffDate, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);

        Action action = () => DispatchMapper.ToDomain(request, shipperId);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void ToResponse_ValidDispatch_ReturnsResponseWithMappedFields()
    {
        var dispatch = Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, new[] { defaultVehicle });

        var response = DispatchMapper.ToResponse(dispatch);

        Assert.Equal(dispatch.DispatchId, response.DispatchId);
        Assert.Equal(dispatch.ShipperId, response.ShipperId);
        Assert.Equal(dispatch.CarrierId, response.CarrierId);
        Assert.Equal(dispatch.Price, response.Price);
        Assert.Equal(dispatch.PickupDate, response.PickupDate);
        Assert.Equal(dispatch.DropoffDate, response.DropoffDate);
        Assert.Equal(dispatch.Description, response.Description);
        Assert.Equal(dispatch.DispatchStatus.ToString(), response.DispatchStatus);
        Assert.Equal(dispatch.IsSigned, response.IsSigned);
        Assert.Equal(dispatch.CreatedAt, response.CreatedAt);
    }

    [Fact]
    public void ToResponse_MapsPickupAndDropoffStop_WithStopNumberAsString()
    {
        var dispatch = Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, new[] { defaultVehicle });

        var response = DispatchMapper.ToResponse(dispatch);

        Assert.Equal(dispatch.PickupStop!.StopId, response.PickupStop!.StopId);
        Assert.Equal("1", response.PickupStop.StopNumber);
        Assert.Equal(dispatch.PickupStop.Address, response.PickupStop.Address);
        Assert.Equal(dispatch.DropoffStop!.StopId, response.DropoffStop!.StopId);
        Assert.Equal("2", response.DropoffStop.StopNumber);
        Assert.Equal(dispatch.DropoffStop.Address, response.DropoffStop.Address);
    }

    [Fact]
    public void ToResponse_MapsVehicles_WithStatusAsStringAndNestedStops()
    {
        var vehicleInputs = new (string? Vin, int Year, string Make, string Model, string? Color)[]
        {
            ("1HGCM82633A1", 2020, "Honda", "Accord", "Blue"),
            ("2HGCM82633A2", 2021, "Toyota", "Corolla", "Red"),
        };
        var dispatch = Dispatch.Create(shipperId, carrierId, price, pickupDate, dropoffDate,
            description, pickupStop, dropoffStop, vehicleInputs);

        var response = DispatchMapper.ToResponse(dispatch);

        var vehicles = dispatch.Vehicles.ToList();
        var vehicleResponses = response.Vehicles.ToList();
        Assert.Equal(vehicles.Count, vehicleResponses.Count);
        for (int i = 0; i < vehicles.Count; i++)
        {
            Assert.Equal(vehicles[i].VehicleId, vehicleResponses[i].VehicleId);
            Assert.Equal(vehicles[i].VehicleStatus.ToString(), vehicleResponses[i].VehicleStatus);
            Assert.Equal(vehicles[i].Vin, vehicleResponses[i].Vin);
            Assert.Equal(vehicles[i].Year, vehicleResponses[i].Year);
            Assert.Equal(vehicles[i].Make, vehicleResponses[i].Make);
            Assert.Equal(vehicles[i].Model, vehicleResponses[i].Model);
            Assert.Equal(vehicles[i].Color, vehicleResponses[i].Color);
            Assert.Equal(vehicles[i].PickupStop.StopId, vehicleResponses[i].PickupStop.StopId);
            Assert.Equal(vehicles[i].DropoffStop.StopId, vehicleResponses[i].DropoffStop.StopId);
        }
    }
}
