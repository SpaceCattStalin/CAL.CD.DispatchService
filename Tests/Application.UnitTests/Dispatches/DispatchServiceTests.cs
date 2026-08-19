using Application.Dispatches;
using Application.UnitTests.TestHelpers;
using Castle.Core.Logging;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

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

    private static UpdateDispatchRequest MakeUpdateRequest(IEnumerable<UpdateVehicleRequest> vehicles) =>
        new(750m, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), "Updated dispatch description",
            new StopRequest("12345 Main Accord Street", "Warehouse A", "John Doe", "555-1234-678", "john@example.com"),
            new StopRequest("45678 Oak Accord Street", "Warehouse B", "Jane Doe", "555-5678-123", "jane@example.com"),
            vehicles);

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

    [Fact]
    public async Task GetBatch_InvalidRequest_ThrowsValidationException()
    {
        var request = new GetDispatchBatchRequest(Array.Empty<Guid>());
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(FailedValidationResult());
        using var db = InMemoryDbContextFactory.Create();
        var service = CreateService(db);

        await Assert.ThrowsAsync<ValidationException>(() => service.GetBatchAsync(request));
    }

    [Fact]
    public async Task GetBatch_AllFound_ReturnsAllInFoundNoneInNotFound()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch1;
        Dispatch dispatch2;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch1 = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            dispatch2 = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.AddRange(dispatch1, dispatch2);
            await seedDb.SaveChangesAsync();
        }

        var request = new GetDispatchBatchRequest(new[] { dispatch1.DispatchId, dispatch2.DispatchId });
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using var db = InMemoryDbContextFactory.Create(dbName);
        var service = CreateService(db);

        var response = await service.GetBatchAsync(request);

        Assert.Equal(2, response.Found.Count());
        Assert.Empty(response.NotFound);
        Assert.Contains(response.Found, d => d.DispatchId == dispatch1.DispatchId);
        Assert.Contains(response.Found, d => d.DispatchId == dispatch2.DispatchId);
    }

    [Fact]
    public async Task GetBatch_MixedFoundAndNotFound_ReturnsCorrectSplit()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        var unknownId = Guid.NewGuid();
        Dispatch dispatch1;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch1 = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.Add(dispatch1);
            await seedDb.SaveChangesAsync();
        }

        var request = new GetDispatchBatchRequest(new[] { dispatch1.DispatchId, unknownId });
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using var db = InMemoryDbContextFactory.Create(dbName);
        var service = CreateService(db);

        var response = await service.GetBatchAsync(request);

        var found = Assert.Single(response.Found);
        Assert.Equal(dispatch1.DispatchId, found.DispatchId);
        var notFound = Assert.Single(response.NotFound);
        Assert.Equal(unknownId, notFound);
    }

    [Fact]
    public async Task GetBatch_AllNotFound_ReturnsAllInNotFound()
    {
        var requestedIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var request = new GetDispatchBatchRequest(requestedIds);
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using var db = InMemoryDbContextFactory.Create();
        var service = CreateService(db);

        var response = await service.GetBatchAsync(request);

        Assert.Empty(response.Found);
        Assert.Equal(requestedIds, response.NotFound.ToArray());
    }

    [Fact]
    public async Task AssignDriver_InvalidRequest_ThrowsValidationException_NoSave()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        var request = new AssignDriverRequest(Guid.Empty);
        mockAssignDriverValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(FailedValidationResult());

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);

            await Assert.ThrowsAsync<ValidationException>(() => service.AssignDriverAsync(dispatch.DispatchId, request));
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches.FindAsync(dispatch.DispatchId);
        Assert.Equal(DispatchStatus.NotSigned, reloaded!.DispatchStatus);
    }

    [Fact]
    public async Task AssignDriver_UnknownDispatch_ThrowsKeyNotFoundException()
    {
        var request = new AssignDriverRequest(Guid.NewGuid());
        mockAssignDriverValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using var db = InMemoryDbContextFactory.Create();
        var service = CreateService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AssignDriverAsync(Guid.NewGuid(), request));
    }

    [Theory]
    [InlineData(true)]  // driver id does not exist in Users at all
    [InlineData(false)] // driver id exists but IsActive is false
    public async Task AssignDriver_UnknownOrInactiveDriver_ThrowsArgumentException(bool driverIsUnknown)
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Dispatch dispatch;
        Guid driverId;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.Add(dispatch);

            if (driverIsUnknown)
            {
                driverId = Guid.NewGuid();
            }
            else
            {
                var inactiveDriver = User.CreateUser(companyId, "Inactive", "Driver", "555-000-2222",
                    "inactive.driver@example.com", "idriver", "hash", UserRole.Driver, isActive: false);
                seedDb.Users.Add(inactiveDriver);
                driverId = inactiveDriver.UserId;
            }

            await seedDb.SaveChangesAsync();
        }

        var request = new AssignDriverRequest(driverId);
        mockAssignDriverValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using var db = InMemoryDbContextFactory.Create(dbName);
        var service = CreateService(db);

        await Assert.ThrowsAsync<ArgumentException>(() => service.AssignDriverAsync(dispatch.DispatchId, request));
    }

    [Fact]
    public async Task AssignDriver_ValidDriver_SetsPendingDeliveryAndSavesOnce()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Dispatch dispatch;
        User driver;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            driver = User.CreateUser(companyId, "Active", "Driver", "555-000-3333",
                "active.driver@example.com", "adriver", "hash", UserRole.Driver);
            seedDb.Dispatches.Add(dispatch);
            seedDb.Users.Add(driver);
            await seedDb.SaveChangesAsync();
        }

        var request = new AssignDriverRequest(driver.UserId);
        mockAssignDriverValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);
            await service.AssignDriverAsync(dispatch.DispatchId, request);
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches.Include(d => d.Drivers).FirstAsync(d => d.DispatchId == dispatch.DispatchId);
        Assert.Equal(DispatchStatus.PendingDelivery, reloaded.DispatchStatus);
        var assignedDriver = Assert.Single(reloaded.Drivers);
        Assert.Equal(driver.UserId, assignedDriver.DriverId);
    }

    [Fact]
    public async Task AssignDriver_AlreadyAssigned_DoesNotDuplicate()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Dispatch dispatch;
        User driver;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            driver = User.CreateUser(companyId, "Active", "Driver", "555-000-4444",
                "active.driver2@example.com", "adriver2", "hash", UserRole.Driver);
            seedDb.Dispatches.Add(dispatch);
            seedDb.Users.Add(driver);
            await seedDb.SaveChangesAsync();
        }

        var request = new AssignDriverRequest(driver.UserId);
        mockAssignDriverValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using (var db1 = InMemoryDbContextFactory.Create(dbName))
        {
            await CreateService(db1).AssignDriverAsync(dispatch.DispatchId, request);
        }

        using (var db2 = InMemoryDbContextFactory.Create(dbName))
        {
            await CreateService(db2).AssignDriverAsync(dispatch.DispatchId, request);
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches.Include(d => d.Drivers).FirstAsync(d => d.DispatchId == dispatch.DispatchId);
        Assert.Single(reloaded.Drivers);
    }

    [Fact]
    public async Task Delete_UnknownDispatch_ThrowsKeyNotFoundException()
    {
        using var db = InMemoryDbContextFactory.Create();
        var service = CreateService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Delete_AlreadyDelivered_ThrowsValidationException_NothingRemoved()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            dispatch.UpdateStatus(DispatchStatus.Delivered);
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);
            await Assert.ThrowsAsync<ValidationException>(() => service.DeleteAsync(dispatch.DispatchId));
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches.Include(d => d.Vehicles).FirstAsync(d => d.DispatchId == dispatch.DispatchId);
        Assert.Equal(DispatchStatus.Delivered, reloaded.DispatchStatus);
        Assert.NotEmpty(reloaded.Vehicles);
    }

    [Fact]
    public async Task Delete_Valid_CancelsAndRemovesStops()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;
        Guid pickupStopId;
        Guid dropoffStopId;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            var pickupStop = MakePickupStop();
            var dropoffStop = MakeDropoffStop();
            pickupStopId = pickupStop.StopId;
            dropoffStopId = dropoffStop.StopId;

            dispatch = MakeDispatch(defaultShipperId, carrierId, pickupStop, dropoffStop);
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);
            await service.DeleteAsync(dispatch.DispatchId);
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches
            .Include(d => d.Vehicles)
            .Include(d => d.Drivers)
            .FirstAsync(d => d.DispatchId == dispatch.DispatchId);

        Assert.Equal(DispatchStatus.Canceled, reloaded.DispatchStatus);
        Assert.Empty(reloaded.Vehicles);
        Assert.Empty(reloaded.Drivers);
        Assert.Null(await verifyDb.Stops.FindAsync(pickupStopId));
        Assert.Null(await verifyDb.Stops.FindAsync(dropoffStopId));
    }

    [Fact]
    public async Task Update_InvalidRequest_ThrowsValidationException_NoSave()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        var request = MakeUpdateRequest(Array.Empty<UpdateVehicleRequest>());
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(FailedValidationResult());

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);
            await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(dispatch.DispatchId, request));
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches.FindAsync(dispatch.DispatchId);
        Assert.Equal(500m, reloaded!.Price);
    }

    [Fact]
    public async Task Update_UnknownDispatch_ThrowsKeyNotFoundException()
    {
        var request = MakeUpdateRequest(new[] { new UpdateVehicleRequest(Guid.Empty, null, 2020, "Honda", "Accord", null) });
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using var db = InMemoryDbContextFactory.Create();
        var service = CreateService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), request));
    }

    [Theory]
    [InlineData(DispatchStatus.Canceled)]
    [InlineData(DispatchStatus.Delivered)]
    [InlineData(DispatchStatus.PendingDelivery)]
    public async Task Update_StatusIsCanceledOrDeliveredOrPendingDelivery_ThrowsValidationException(DispatchStatus status)
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            dispatch.UpdateStatus(status);
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        var request = MakeUpdateRequest(new[] { new UpdateVehicleRequest(Guid.Empty, null, 2020, "Honda", "Accord", null) });
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using var db = InMemoryDbContextFactory.Create(dbName);
        var service = CreateService(db);

        await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(dispatch.DispatchId, request));
    }

    [Fact]
    public async Task Update_ExistingVehicleIdOnThisDispatch_UpdatesFieldsInPlace()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        var existingVehicleId = dispatch.Vehicles.Single().VehicleId;
        var request = MakeUpdateRequest(new[]
        {
            new UpdateVehicleRequest(existingVehicleId, "9HGCM82633AUPDATE", 2019, "Ford", "Fusion", "Red")
        });
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);
            await service.UpdateAsync(dispatch.DispatchId, request);
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches.Include(d => d.Vehicles).FirstAsync(d => d.DispatchId == dispatch.DispatchId);
        var vehicle = Assert.Single(reloaded.Vehicles);
        Assert.Equal(existingVehicleId, vehicle.VehicleId);
        Assert.Equal("9HGCM82633AUPDATE", vehicle.Vin);
        Assert.Equal(2019, vehicle.Year);
        Assert.Equal("Ford", vehicle.Make);
        Assert.Equal("Fusion", vehicle.Model);
        Assert.Equal("Red", vehicle.Color);
    }

    [Fact]
    public async Task Update_VehicleIdBelongsToAnotherDispatch_ThrowsArgumentException()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatchA;
        Dispatch dispatchB;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatchA = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            dispatchB = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.AddRange(dispatchA, dispatchB);
            await seedDb.SaveChangesAsync();
        }

        var otherDispatchVehicleId = dispatchB.Vehicles.Single().VehicleId;
        var request = MakeUpdateRequest(new[]
        {
            new UpdateVehicleRequest(otherDispatchVehicleId, null, 2020, "Honda", "Accord", null)
        });
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using var db = InMemoryDbContextFactory.Create(dbName);
        var service = CreateService(db);

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(dispatchA.DispatchId, request));
    }

    [Fact]
    public async Task Update_UnknownVehicleId_CreatesNewVehicle()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        var existingVehicleId = dispatch.Vehicles.Single().VehicleId;
        var unknownVehicleId = Guid.NewGuid();
        var request = MakeUpdateRequest(new[]
        {
            new UpdateVehicleRequest(existingVehicleId, null, 2020, "Honda", "Accord", null),
            new UpdateVehicleRequest(unknownVehicleId, "1HGCM82633ANEW1", 2022, "Toyota", "Camry", "Silver")
        });
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);
            await service.UpdateAsync(dispatch.DispatchId, request);
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches.Include(d => d.Vehicles).FirstAsync(d => d.DispatchId == dispatch.DispatchId);
        Assert.Equal(2, reloaded.Vehicles.Count);
        var created = Assert.Single(reloaded.Vehicles, v => v.VehicleId != existingVehicleId);
        Assert.Equal("Toyota", created.Make);
        Assert.Equal("Camry", created.Model);
        Assert.NotEqual(unknownVehicleId, created.VehicleId);
    }

    [Fact]
    public async Task Update_VehicleOmittedFromRequest_IsRemoved()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        var originalVehicleId = dispatch.Vehicles.Single().VehicleId;
        var request = MakeUpdateRequest(new[]
        {
            new UpdateVehicleRequest(Guid.NewGuid(), null, 2021, "Nissan", "Altima", null)
        });
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);
            await service.UpdateAsync(dispatch.DispatchId, request);
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches.Include(d => d.Vehicles).FirstAsync(d => d.DispatchId == dispatch.DispatchId);
        Assert.Single(reloaded.Vehicles);
        Assert.DoesNotContain(reloaded.Vehicles, v => v.VehicleId == originalVehicleId);
        Assert.Null(await verifyDb.Vehicles.FindAsync(originalVehicleId));
    }

    [Fact]
    public async Task Update_UpdatesStopsAndDispatchFields()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        var existingVehicleId = dispatch.Vehicles.Single().VehicleId;
        var request = new UpdateDispatchRequest(
            999m, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(6), "New description",
            new StopRequest("12345 Main Accord Street Updated", "Warehouse A2", "New Contact", "555-000-9999", "new@example.com"),
            new StopRequest("45678 Oak Accord Street Updated", "Warehouse B2", "New Contact 2", "555-000-8888", "new2@example.com"),
            new[] { new UpdateVehicleRequest(existingVehicleId, null, 2020, "Honda", "Accord", null) });
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);
            await service.UpdateAsync(dispatch.DispatchId, request);
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches
            .Include(d => d.PickupStop)
            .Include(d => d.DropoffStop)
            .FirstAsync(d => d.DispatchId == dispatch.DispatchId);

        Assert.Equal(999m, reloaded.Price);
        Assert.Equal("New description", reloaded.Description);
        Assert.Equal("12345 Main Accord Street Updated", reloaded.PickupStop!.Address);
        Assert.Equal("45678 Oak Accord Street Updated", reloaded.DropoffStop!.Address);
    }

    [Fact]
    public async Task Update_Valid_CallsSaveChangesAsyncOnce()
    {
        var dbName = Guid.NewGuid().ToString();
        var carrierId = Guid.NewGuid();
        Dispatch dispatch;

        using (var seedDb = InMemoryDbContextFactory.Create(dbName))
        {
            dispatch = MakeDispatch(defaultShipperId, carrierId, MakePickupStop(), MakeDropoffStop());
            seedDb.Dispatches.Add(dispatch);
            await seedDb.SaveChangesAsync();
        }

        var existingVehicleId = dispatch.Vehicles.Single().VehicleId;
        var request = MakeUpdateRequest(new[]
        {
            new UpdateVehicleRequest(existingVehicleId, null, 2020, "Honda", "Accord", null)
        });
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using (var db = InMemoryDbContextFactory.Create(dbName))
        {
            var service = CreateService(db);
            var response = await service.UpdateAsync(dispatch.DispatchId, request);
            Assert.Equal(750m, response.Price);
        }

        using var verifyDb = InMemoryDbContextFactory.Create(dbName);
        var reloaded = await verifyDb.Dispatches.FindAsync(dispatch.DispatchId);
        Assert.Equal(750m, reloaded!.Price);
    }
}
