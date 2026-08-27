using Application.Dispatches;
using Application.Dispatches.Validator;

namespace Application.UnitTests.Dispatches;

public class CreateDispatchRequestValidatorTests
{
    private readonly CreateDispatchRequestValidator validator = new();
    private static CreateDispatchRequest ValidRequest() => new(
        Guid.NewGuid(),
        100m,
        DateTime.UtcNow.AddDays(1),
        DateTime.UtcNow.AddDays(2),
        "Testing",
        new StopRequest(
            "123 Main Street, Xo Viet Nghe Tinh",
            "Phuong So 27",
            "John Overwatch",
            "555-123-4567",
            "john@example.com"
        ),
        new StopRequest(
            "123 Main Street, Le Van Viet",
            "Phuong So 27",
            "John Valorant",
            "555-123-4589",
            "johnny@example.com"
        ),
        new[] { new VehicleRequest("1HGC13232141", 2020, "Honda", "Accord", "Blue") }
    );

    private static VehicleRequest[] MakeVehicleRequest(int count) =>
        Enumerable.Range(0, count)
        .Select(x => new VehicleRequest("1HGC13232141", 2020, "Honda", "Accord", "Blue"))
        .ToArray();

    [Fact]
    public void Validate_ValidRequest_IsValid()
    {
        var request = ValidRequest();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyCarrierId_IsInvalid()
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(Guid.Empty, baseRequest.Price, baseRequest.PickupDate, baseRequest.DropoffDate, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName.Equals("CarrierId"));
    }

    [Fact]
    public void Validate_PriceZero_IsInvalid()
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, 0, baseRequest.PickupDate, baseRequest.DropoffDate, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName.Equals("Price"));
    }

    [Fact]
    public void Validate_SmallestPositivePrice_IsValid()
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, 0.01m, baseRequest.PickupDate, baseRequest.DropoffDate, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);
        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PriceNegative_IsInvalid()
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, -100, baseRequest.PickupDate, baseRequest.DropoffDate, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName.Equals("Price"));
    }

    [Fact]
    public void Validate_PickupDateInPast_IsInvalid()
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, DateTime.UtcNow.AddDays(-10), baseRequest.DropoffDate, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName.Equals("PickupDate"));
    }

    [Fact]
    public void Validate_PickupDateNow_Valid()
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, DateTime.UtcNow, baseRequest.DropoffDate, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }


    [Fact]
    public void Validate_PickupDateInFuture_IsValid()
    {
        var pickup = DateTime.UtcNow.AddDays(10);
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, pickup, pickup.AddDays(1), baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, -1)]
    public void Validate_DropoffDateNotAfterPickupOrNow_IsInvalid(int pickupOffsetDays, int dropoffOffsetHoursFromPickup)
    {
        var pickup = DateTime.UtcNow.AddDays(2 + pickupOffsetDays);
        var dropoff = pickup.AddHours(dropoffOffsetHoursFromPickup);
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, pickup, dropoff, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_DropoffAfterPickupAndNow_IsValid()
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);
        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(500, true)]
    [InlineData(501, false)]
    public void Validate_DescriptionLength_RespectsMaxLength(int length, bool expectedValid)
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, baseRequest.PickupDate, baseRequest.DropoffDate, new string('a', length), baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);

        var result = validator.Validate(request);

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void Validate_NullDescription_IsValid()
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, baseRequest.PickupDate, baseRequest.DropoffDate, null, baseRequest.PickupStop, baseRequest.DropoffStop, baseRequest.Vehicles);
        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(12)]
    public void Validate_VehicleCountWithinRange_IsValid(int count)
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, baseRequest.PickupDate, baseRequest.DropoffDate, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, MakeVehicleRequest(count));

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Validate_VehicleCountOutOfRange_IsValid(int count)
    {
        var baseRequest = ValidRequest();
        var request = new CreateDispatchRequest(baseRequest.CarrierId, baseRequest.Price, baseRequest.PickupDate, baseRequest.DropoffDate, baseRequest.Description, baseRequest.PickupStop, baseRequest.DropoffStop, MakeVehicleRequest(count));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
