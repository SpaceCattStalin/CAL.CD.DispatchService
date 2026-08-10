using Domain;

namespace Application.UnitTests.Domain;

public class StopTests
{
    private const string address = "123 Main Street, Xo Viet Nghe Tinh";
    private const string locationName = "Phuong 27";
    private const string contactName = "John Overwatch";
    private const string contactPhone = "555-123-4567";
    private const string contactEmail = "john@example.com";

    [Fact]
    public void CreateStop_ValidInputs_ReturnsStopWithDefaults()
    {
        var stop = Stop.Create(1, address, locationName, contactName, contactPhone, contactEmail);

        Assert.NotEqual(Guid.Empty, stop.StopId);
        Assert.Equal(1, stop.StopNumber);
        Assert.Equal(address, stop.Address);
        Assert.Equal(locationName, stop.LocationName);
        Assert.Equal(contactName, stop.ContactName);
        Assert.Equal(contactPhone, stop.ContactPhone);
        Assert.Equal(contactEmail, stop.ContactEmail);
    }

    [Fact]
    public void CreateStop_EmptyAddress_ThrowsArgumentException()
    {
        Action action = () => Stop.Create(1, "", locationName, contactName, contactPhone, contactEmail);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void CreateStop_NullLocationName_DoesNotThrowsArgumentException()
    {
        var stop = Stop.Create(1, address, null, contactName, contactPhone, contactEmail);

        Assert.Null(stop.LocationName);
    }

    [Fact]
    public void CreateStop_NullContactName_DoesNotThrowsArgumentException()
    {
        var stop = Stop.Create(1, address, locationName, null, contactPhone, contactEmail);

        Assert.Null(stop.ContactName);
    }

    [Fact]
    public void CreateStop_NullContactPhone_DoesNotThrowsArgumentException()
    {
        var stop = Stop.Create(1, address, locationName, contactName, null, contactEmail);

        Assert.Null(stop.ContactPhone);
    }

    [Fact]
    public void CreateStop_NullContactEmail_DoesNotThrowsArgumentException()
    {
        var stop = Stop.Create(1, address, locationName, contactName, contactPhone, null);

        Assert.Null(stop.ContactEmail);
    }

    [Fact]
    public void CreateStop_StopNumberTwo_ReturnsStopWithStopNumberTwo()
    {
        var stop = Stop.Create(2, address, locationName, contactName, contactPhone, contactEmail);

        Assert.Equal(2, stop.StopNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-1)]
    public void CreateStop_InvalidStopNumber_ThrowsArgumentException(int stopNumber)
    {
        Action action = () => Stop.Create(stopNumber, address, locationName, contactName, contactPhone, contactEmail);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}
