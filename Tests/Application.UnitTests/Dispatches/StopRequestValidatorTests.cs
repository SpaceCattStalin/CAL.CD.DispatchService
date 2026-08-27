namespace Application.UnitTests.Dispatches;

using Application.Dispatches;
using Application.Dispatches.Validator;


public class StopRequestValidatorTests
{
    private readonly StopRequestValidator validator = new();

    private static StopRequest ValidRequest() => new(
        "123 Main Street, Xo Viet Nghe Tinh",
        "Phuong So 27",
        "John Overwatch",
        "555-123-4567",
        "john@example.com");

    private static string MakeValidEmail(int length)
    {
        string suffix = "@a.com";
        return new string('a', length - suffix.Length) + suffix;
    }

    [Fact]
    public void Validate_ValidRequest_IsValid()
    {
        var request = ValidRequest();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(100)]
    public void Validate_AddressLengthWithinRange_IsValid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(new string('a', length), baseRequest.LocationName, baseRequest.ContactName, baseRequest.ContactPhone, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(19)]
    [InlineData(101)]
    public void Validate_AddressLengthOutOfRange_IsInvalid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(new string('a', length), baseRequest.LocationName, baseRequest.ContactName, baseRequest.ContactPhone, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_AddressEmpty_IsInvalid(string? address)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(address!, baseRequest.LocationName, baseRequest.ContactName, baseRequest.ContactPhone, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_NullLocationName_IsValid()
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, null, baseRequest.ContactName, baseRequest.ContactPhone, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(30)]
    public void Validate_LocationNameLengthWithinRange_IsValid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, new string('a', length), baseRequest.ContactName, baseRequest.ContactPhone, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(31)]
    public void Validate_LocationNameLengthOutOfRange_IsInvalid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, new string('a', length), baseRequest.ContactName, baseRequest.ContactPhone, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_NullContactName_IsValid()
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, baseRequest.LocationName, null, baseRequest.ContactPhone, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(50)]
    public void Validate_ContactNameLengthWithinRange_IsValid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, baseRequest.LocationName, new string('a', length), baseRequest.ContactPhone, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(51)]
    public void Validate_ContactNameLengthOutOfRange_IsInvalid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, baseRequest.LocationName, new string('a', length), baseRequest.ContactPhone, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_NullContactPhone_IsValid()
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, baseRequest.LocationName, baseRequest.ContactName, null, baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(12)]
    public void Validate_ContactPhoneLengthWithinRange_IsValid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, baseRequest.LocationName, baseRequest.ContactName, new string('1', length), baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(13)]
    public void Validate_ContactPhoneLengthOutOfRange_IsInvalid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, baseRequest.LocationName, baseRequest.ContactName, new string('1', length), baseRequest.ContactEmail);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_NullContactEmail_IsValid()
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, baseRequest.LocationName, baseRequest.ContactName, baseRequest.ContactPhone, null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(15)]
    [InlineData(30)]
    public void Validate_ContactEmailLengthWithinRange_IsValid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, baseRequest.LocationName, baseRequest.ContactName, baseRequest.ContactPhone, MakeValidEmail(length));

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(14)]
    [InlineData(31)]
    public void Validate_ContactEmailLengthOutOfRange_IsInvalid(int length)
    {
        var baseRequest = ValidRequest();
        var request = new StopRequest(baseRequest.Address, baseRequest.LocationName, baseRequest.ContactName, baseRequest.ContactPhone, MakeValidEmail(length));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
