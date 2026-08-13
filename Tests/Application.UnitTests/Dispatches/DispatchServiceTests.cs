using Application.Dispatches;
using Application.UnitTests.TestHelpers;
using Castle.Core.Logging;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;

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
    private readonly Mock<IValidator<AssignDriverRequest>> mockAssignDriverValidator = new();
    private readonly Mock<IValidator<UpdateDispatchRequest>> mockUpdateValidator = new();
    private readonly Mock<ILogger<DispatchService>> mockLogger = new();
    private readonly Mock<ICurrentUserService> mockCurrentUser = new();
    private readonly Mock<DbSet<Dispatch>> mockSet = new();
    private readonly Mock<IApplicationDbContext> mockDb = new();

    private DispatchService CreateService()
    {
        mockDb.Setup(db => db.Dispatches).Returns(mockSet.Object);
        mockDb.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);
        mockCurrentUser.Setup(u => u.UserId).Returns(defaultShipperId);
        return new DispatchService(mockDb.Object, mockLogger.Object, mockValidator.Object, mockBatchValidator.Object, mockAssignDriverValidator.Object, mockUpdateValidator.Object, mockCurrentUser.Object);
    }

    // Methods below need real Include()/FirstOrDefaultAsync() query support that
    // Mock<DbSet<T>> cannot provide, so they run against a real (InMemory) IApplicationDbContext.
    private DispatchService CreateService(IApplicationDbContext db)
    {
        mockCurrentUser.Setup(u => u.UserId).Returns(defaultShipperId);
        return new DispatchService(db, mockLogger.Object, mockValidator.Object, mockBatchValidator.Object, mockAssignDriverValidator.Object, mockUpdateValidator.Object, mockCurrentUser.Object);
    }

    private static Stop MakePickupStop() => Stop.Create(1,
        "12345 Main Accord Street", "Warehouse A", "John Doe", "555-1234-678", "john@example.com");

    private static Stop MakeDropoffStop() => Stop.Create(2,
        "45678 Oak Accord Street", "Warehouse B", "Jane Doe", "555-5678-123", "jane@example.com");

    private static Dispatch MakeDispatch(Guid shipperId, Guid carrierId, Stop pickupStop, Stop dropoffStop) =>
        Dispatch.Create(shipperId, carrierId, 500m, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            "Test dispatch", pickupStop, dropoffStop,
            new (string? Vin, int Year, string Make, string Model, string? Color)[]
            {
                ("1HGCM82633A123", 2020, "Honda", "Accord", "Blue")
            });

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
        Assert.Equal(request.PickupStop.Address, response.PickupStop!.Address);
        Assert.Equal(request.DropoffStop.Address, response.DropoffStop!.Address);
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

    [Fact]
    public async Task GetById_ExistingDispatchWithChildren_ReturnsFullyMappedResponse()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            var dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            var driver = User.CreateUser(companyId, "John", "Doe", "555-000-1111",
                "john.doe@example.com", "jdoe", "hash", UserRole.Driver);

            dispatch.Drivers.Add(new DispatchDriver { DispatchId = dispatch.DispatchId, DriverId = driver.UserId });

            seedDb.Dispatches.Add(dispatch);
            seedDb.Users.Add(driver);
            await seedDb.SaveChangesAsync();

            using var db = InMemoryDbContextFactory.Create(dbName);
            var service = CreateService(db);

            var response = await service.GetByIdAsync(dispatch.DispatchId);

            Assert.Equal(dispatch.DispatchId, response.DispatchId);
            Assert.Equal("12345 Main Accord Street", response.PickupStop!.Address);
            Assert.Equal("45678 Oak Accord Street", response.DropoffStop!.Address);
            Assert.Single(response.Vehicles);
            Assert.Equal("Honda", response.Vehicles.Single().Make);
            var responseDriver = Assert.Single(response.Drivers);
            Assert.Equal("John", responseDriver.FirstName);
            Assert.Equal("Doe", responseDriver.LastName);
        }
    }

    [Fact]
    public async Task GetById_UnknownId_ThrowsKeyNotFoundException()
    {
        using var db = InMemoryDbContextFactory.Create();
        var service = CreateService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByIdAsync(Guid.NewGuid()));
    }
}
