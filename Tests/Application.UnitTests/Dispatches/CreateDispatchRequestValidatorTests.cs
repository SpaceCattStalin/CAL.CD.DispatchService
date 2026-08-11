using Application.Dispatches;

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
            "Phuong 27",
            "John Overwatch",
            "555-123-4567",
            "john@example.com"
        ),
        new StopRequest(
            "123 Main Street, Le Van Viet",
            "Phuong 27",
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
        var request = ValidRequest() with { CarrierId = Guid.Empty };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName.Equals("CarrierId"));
    }

    [Fact]
    public void Validate_PriceZero_IsInvalid()
    {
        var request = ValidRequest() with { Price = 0 };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName.Equals("Price"));
    }

    [Fact]
    public void Validate_SmallestPositivePrice_IsValid()
    {
        var request = ValidRequest() with { Price = 0.01m };
        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PriceNegative_IsInvalid()
    {
        var request = ValidRequest() with { Price = -100 };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName.Equals("Price"));
    }

    [Fact]
    public void Validate_PickupDateInPast_IsInvalid()
    {
        var request = ValidRequest() with { PickupDate = DateTime.UtcNow.AddDays(-10) };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName.Equals("PickupDate"));
    }

    [Fact]
    public void Validate_PickupDateNow_Valid()
    {
        var request = ValidRequest() with { PickupDate = DateTime.UtcNow };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }


    [Fact]
    public void Validate_PickupDateInFuture_IsValid()
    {
        var pickup = DateTime.UtcNow.AddDays(10);
        var request = ValidRequest() with { PickupDate = pickup, DropoffDate = pickup.AddDays(1) };

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
        var request = ValidRequest() with { PickupDate = pickup, DropoffDate = dropoff };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_DropoffAfterPickupAndNow_IsValid()
    {
        var request = ValidRequest() with { PickupDate = DateTime.UtcNow.AddDays(1), DropoffDate = DateTime.UtcNow.AddDays(2) };
        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(500, true)]
    [InlineData(501, false)]
    public void Validate_DescriptionLength_RespectsMaxLength(int length, bool expectedValid)
    {
        var request = ValidRequest() with { Description = new string('a', length) };

        var result = validator.Validate(request);

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void Validate_NullDescription_IsValid()
    {
        var request = ValidRequest() with { Description = null };
        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(12)]
    public void Validate_VehicleCountWithinRange_IsValid(int count)
    {
        var request = ValidRequest() with { Vehicles = MakeVehicleRequest(count) };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Validate_VehicleCountOutOfRange_IsValid(int count)
    {
        var request = ValidRequest() with { Vehicles = MakeVehicleRequest(count) };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
