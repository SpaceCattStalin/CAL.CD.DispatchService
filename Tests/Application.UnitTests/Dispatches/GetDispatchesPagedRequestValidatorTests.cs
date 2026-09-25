using Application.Dispatches;
using Application.Dispatches.Validator;

namespace Application.UnitTests.Dispatches;

public class GetDispatchesPagedRequestValidatorTests
{
    private readonly GetDispatchesPagedRequestValidator validator = new();

    private static bool HasErrorFor(FluentValidation.Results.ValidationResult result, string propertyName) =>
        result.Errors.Any(e => e.PropertyName == propertyName);

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(500, true)]
    [InlineData(501, false)]
    public void Validate_Limit_MustBeBetween1And500(int limit, bool isValid)
    {
        var result = validator.Validate(new GetDispatchesPagedRequest(null, limit));

        Assert.Equal(!isValid, HasErrorFor(result, nameof(GetDispatchesPagedRequest.Limit)));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("not-a-guid", false)]
    [InlineData("3fa85f64-5717-4562-b3fc-2c963f66afa6", true)]
    public void Validate_Cursor_MustBeEmptyOrGuid(string? cursor, bool isValid)
    {
        var result = validator.Validate(new GetDispatchesPagedRequest(cursor, 10));

        Assert.Equal(!isValid, HasErrorFor(result, nameof(GetDispatchesPagedRequest.Cursor)));
    }
}
