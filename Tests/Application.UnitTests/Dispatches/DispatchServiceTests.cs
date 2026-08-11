using Application.Dispatches;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.UnitTests.Dispatches;

public class DispatchServiceTests
{
    private readonly Guid defaultShipperId = Guid.NewGuid();

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

    private static CreateDispatchRequest MakeRequest() =>
        new(Guid.NewGuid(), 100m, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            "Test dispatch", pickupStopRequest, dropoffStopRequest, new[] { defaultVehicleRequest });

    private static ValidationResult SuccessfulValidationResult() => new();

    private static ValidationResult FailedValidationResult() =>
        new(new[] { new ValidationFailure("CarrierId", "CarrierId is required") });

    private readonly Mock<IValidator<CreateDispatchRequest>> mockValidator = new();
    private readonly Mock<IValidator<GetDispatchBatchRequest>> mockBatchValidator = new();
    private readonly Mock<ICurrentUserService> mockCurrentUser = new();
    private readonly Mock<DbSet<Dispatch>> mockSet = new();
    private readonly Mock<IApplicationDbContext> mockDb = new();

    private DispatchService CreateService()
    {
        mockDb.Setup(db => db.Dispatches).Returns(mockSet.Object);
        mockDb.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);
        mockCurrentUser.Setup(u => u.UserId).Returns(defaultShipperId);
        return new DispatchService(mockDb.Object, mockValidator.Object, mockBatchValidator.Object, mockCurrentUser.Object);
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsValidationException_AndDoesNotSave()
    {
        var request = MakeRequest();
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(FailedValidationResult());
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(request));

        mockSet.Verify(s => s.Add(It.IsAny<Dispatch>()), Times.Never);
        mockDb.Verify(db => db.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_AddsDispatchWithCurrentShipperId()
    {
        var request = MakeRequest();
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());
        var service = CreateService();

        await service.CreateAsync(request);

        mockSet.Verify(s => s.Add(It.Is<Dispatch>(d =>
            d.ShipperId == defaultShipperId &&
            d.CarrierId == request.CarrierId &&
            d.Price == request.Price)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsSaveChangesAsyncOnce()
    {
        var request = MakeRequest();
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());
        var service = CreateService();

        await service.CreateAsync(request);

        mockDb.Verify(db => db.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsResponseMatchingDispatch()
    {
        var request = MakeRequest();
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());
        var service = CreateService();

        var response = await service.CreateAsync(request);

        Assert.NotEqual(Guid.Empty, response.DispatchId);
        Assert.Equal(defaultShipperId, response.ShipperId);
        Assert.Equal("NotSigned", response.DispatchStatus);
        Assert.Equal(request.Vehicles.Count(), response.Vehicles.Count());
        Assert.Equal(request.PickupStop.Address, response.PickupStop.Address);
        Assert.Equal(request.DropoffStop.Address, response.DropoffStop.Address);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsValidatorWithGivenRequest()
    {
        var request = MakeRequest();
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());
        var service = CreateService();

        await service.CreateAsync(request);

        mockValidator.Verify(v => v.ValidateAsync(request, default), Times.Once);
    }
}
