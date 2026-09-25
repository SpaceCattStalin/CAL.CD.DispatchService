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
using Application.Events;
using Infrastructure;
using System.Data;
using Microsoft.AspNetCore.Http.Features;

namespace Application.UnitTests.Dispatches;

public class DispatchServiceTests
{
    private static readonly Guid shipperOwnerId = Guid.Parse("30000000-0000-0000-0000-000000000002");
    private static readonly Guid carrierOwnerId = Guid.Parse("51000000-0000-0000-0000-000000000003");
    private static readonly CompanyType shipperCompanyType = CompanyType.Shipper;
    private static readonly CompanyType carrierCompanyType = CompanyType.Carrier;
    private static readonly Guid shipperCompanyId = Guid.Parse("30000000-0000-0000-0000-000000000001");
    private static readonly Guid carrierCompanyId = Guid.Parse("50000000-0000-0000-0000-000000000003");
    private static readonly DateTime pickupDate = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime dropoffDate = DateTime.UtcNow.AddDays(2);
    private static ValidationResult SuccessfulValidationResult() => new();

    private static ValidationResult FailedValidationResult() =>
        new(new[] { new ValidationFailure("Placeholder", "Placeholder") });

    private readonly Mock<IValidator<CreateDispatchRequest>> mockValidator = new();
    private readonly Mock<IValidator<GetDispatchBatchRequest>> mockBatchValidator = new();
    private readonly Mock<IValidator<AssignDriverRequest>> mockAssignDriverValidator = new();
    private readonly Mock<IValidator<UpdateDispatchRequest>> mockUpdateValidator = new();
    private readonly Mock<IValidator<GetDispatchesPagedRequest>> mockPagedValidator = new();
    private readonly Mock<ILogger<DispatchService>> mockLogger = new();
    private readonly Mock<ICurrentUserService> mockCurrentUser = new();
    private readonly Mock<DbSet<Dispatch>> mockSet = new();
    private readonly Mock<IApplicationDbContext> mockDb = new();
    private readonly Mock<IEventPublisher> mockPublisher = new();

    private DispatchService CreateServiceForShipper(IApplicationDbContext db)
    {
        mockCurrentUser.Setup(u => u.UserId).Returns(shipperOwnerId);
        mockCurrentUser.Setup(u => u.CompanyId).Returns(shipperCompanyId);
        mockCurrentUser.Setup(u => u.CompanyType).Returns(shipperCompanyType);


        return new DispatchService(db, mockLogger.Object, mockValidator.Object, mockBatchValidator.Object, mockAssignDriverValidator.Object, mockUpdateValidator.Object, mockPagedValidator.Object, mockPublisher.Object, mockCurrentUser.Object);
    }

    private DispatchService CreateServiceForCarrier(IApplicationDbContext db)
    {
        mockCurrentUser.Setup(u => u.UserId).Returns(carrierOwnerId);
        mockCurrentUser.Setup(u => u.CompanyId).Returns(carrierCompanyId);
        mockCurrentUser.Setup(u => u.CompanyType).Returns(carrierCompanyType);

        return new DispatchService(db, mockLogger.Object, mockValidator.Object, mockBatchValidator.Object, mockAssignDriverValidator.Object, mockUpdateValidator.Object, mockPagedValidator.Object, mockPublisher.Object, mockCurrentUser.Object);
    }

    private static CreateDispatchRequest MakeRequest(Guid carrierCompanyId) =>
        new(carrierCompanyId, 100m, pickupDate, dropoffDate,
            "Test dispatch", pickupStopRequest, dropoffStopRequest, new[] { defaultVehicleRequest });

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

    private static UpdateDispatchRequest MakeUpdateRequest(IEnumerable<UpdateVehicleRequest> vehicles) =>
        new(750m, pickupDate, dropoffDate, "Updated dispatch description",
            new StopRequest("12345 Main Accord Street", "Warehouse A", "John Doe", "555-1234-678", "john@example.com"),
            new StopRequest("45678 Oak Accord Street", "Warehouse B", "Jane Doe", "555-5678-123", "jane@example.com"),
            vehicles);

    private static void AssertStopMatchesRequest(StopRequest expected, Stop? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.Address, actual.Address);
        Assert.Equal(expected.LocationName, actual.LocationName);
        Assert.Equal(expected.ContactName, actual.ContactName);
        Assert.Equal(expected.ContactPhone, actual.ContactPhone);
        Assert.Equal(expected.ContactEmail, actual.ContactEmail);
    }

    private static void AssertStopMatches(Stop? expected, StopResponse? actual)
    {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        Assert.Equal(expected.StopId, actual.StopId);
        Assert.Equal(expected.StopNumber.ToString(), actual.StopNumber);
        Assert.Equal(expected.Address, actual.Address);
        Assert.Equal(expected.LocationName, actual.LocationName);
        Assert.Equal(expected.ContactName, actual.ContactName);
        Assert.Equal(expected.ContactPhone, actual.ContactPhone);
        Assert.Equal(expected.ContactEmail, actual.ContactEmail);
    }

    private static void AssertCompanyMatches(Company? expected, CompanyResponse? actual)
    {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        Assert.Equal(expected.CompanyId, actual.CompanyId);
        Assert.Equal(expected.CompanyName, actual.CompanyName);
        Assert.Equal(expected.CompanyPhone, actual.CompanyPhone);
        Assert.Equal(expected.CompanyEmail, actual.CompanyEmail);
    }

    private static void AssertVehicleMatches(Vehicle expected, VehicleResponse actual)
    {
        Assert.Equal(expected.VehicleId, actual.VehicleId);
        Assert.Equal(expected.VehicleStatus.ToString(), actual.VehicleStatus);
        Assert.Equal(expected.Vin, actual.Vin);
        Assert.Equal(expected.Year, actual.Year);
        Assert.Equal(expected.Make, actual.Make);
        Assert.Equal(expected.Model, actual.Model);
        Assert.Equal(expected.Color, actual.Color);
        AssertStopMatches(expected.PickupStop, actual.PickupStop);
        AssertStopMatches(expected.DropoffStop, actual.DropoffStop);
    }

    private static void AssertDriverMatches(User expected, DriverResponse actual)
    {
        Assert.Equal(expected.UserId, actual.DriverId);
        Assert.Equal(expected.FirstName, actual.FirstName);
        Assert.Equal(expected.LastName, actual.LastName);
        Assert.Equal(expected.Phone, actual.Phone);
        Assert.Equal(expected.Email, actual.Email);
    }

    // The database does not guarantee the order of a collection, so match each item by id
    private static void AssertVehiclesMatch(IEnumerable<Vehicle> expected, IEnumerable<VehicleResponse> actual)
    {
        var expectedList = expected.ToList();
        var actualList = actual.ToList();
        Assert.Equal(expectedList.Count, actualList.Count);

        foreach (var vehicle in expectedList)
            AssertVehicleMatches(vehicle, Assert.Single(actualList, v => v.VehicleId == vehicle.VehicleId));
    }

    private static void AssertDriversMatch(IEnumerable<DispatchDriver> expected, IEnumerable<DriverResponse> actual)
    {
        var expectedList = expected.ToList();
        var actualList = actual.ToList();
        Assert.Equal(expectedList.Count, actualList.Count);

        foreach (var dispatchDriver in expectedList)
            AssertDriverMatches(dispatchDriver.Driver, Assert.Single(actualList, d => d.DriverId == dispatchDriver.DriverId));
    }

    // Clear the change tracker first so the dispatch is read back from the database,
    // not from the entities the service changed in memory
    private static async Task<Dispatch> ReloadDispatchAsync(ApplicationDbContext db, Guid dispatchId)
    {
        db.ChangeTracker.Clear();
        return await db.Dispatches
            .Include(d => d.Vehicles)
            .Include(d => d.Drivers)
            .Include(d => d.PickupStop)
            .Include(d => d.DropoffStop)
            .SingleAsync(d => d.DispatchId == dispatchId);
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsValidationException_AndDoesNotSave()
    {
        // Setup
        var request = MakeRequest(carrierCompanyId);
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(FailedValidationResult());
        var publishedEvents = new List<DispatchWriterEvent>();
        mockPublisher.Setup(p => p.Publish(It.IsAny<DispatchWriterEvent>()))
                    .Callback<DispatchWriterEvent>(e => publishedEvents.Add(e))
                    .Returns(Task.CompletedTask);
        using var db = InMemoryDbContextFactory.Create();
        // No need to seed data because this test case will fail either way
        var service = CreateServiceForShipper(db);

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(request));

        db.ChangeTracker.Clear();
        Assert.Empty(db.Dispatches);
        Assert.Empty(publishedEvents);
    }


    [Fact]
    public async Task CreateAsync_ValidRequest_AddsDispatchWithOwner()
    {
        // Setup
        var request = MakeRequest(carrierCompanyId);
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());
        var publishedEvents = new List<DispatchWriterEvent>();
        mockPublisher.Setup(p => p.Publish(It.IsAny<DispatchWriterEvent>()))
                    .Callback<DispatchWriterEvent>(e => publishedEvents.Add(e))
                    .Returns(Task.CompletedTask);
        // Create an instance of local Db context 
        using var db = InMemoryDbContextFactory.Create();
        // Add the Owner of the Shipper company
        await db.SeedUserAsync(shipperOwnerId, shipperCompanyId, UserRole.Owner);
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        // Clear the db context after seeding the context
        // to simulate the case where each http request is a seperate db context
        db.ChangeTracker.Clear();

        var service = CreateServiceForShipper(db);
        var result = await service.CreateAsync(request);
        // Clear the db context after creating to clear all the dispatch information from memory (RAM)
        // to simulate the case where each http request is a seperate db context
        // data will be fetch from the storage (the in memory database) while the db context memory will be cleared
        db.ChangeTracker.Clear();
        // Call Clear without Include will trigger the Stop and Vehicle test to fail 
        var dispatch = Assert.Single(db.Dispatches
            .Include(d => d.PickupStop)
            .Include(d => d.DropoffStop)
            .Include(d => d.Vehicles)
            );
        Assert.Equal(shipperCompanyId, dispatch.ShipperId);
        Assert.Equal(carrierCompanyId, dispatch.CarrierId);
        Assert.Equal(DispatchStatus.NotSigned, dispatch.DispatchStatus);
        Assert.Equal("Test dispatch", dispatch.Description);
        Assert.Equal(dispatch.CreatedAt, result.CreatedAt);
        Assert.Equal(pickupDate, dispatch.PickupDate);
        Assert.Equal(dropoffDate, dispatch.DropoffDate);

        AssertStopMatchesRequest(request.PickupStop, dispatch.PickupStop);
        AssertStopMatchesRequest(request.DropoffStop, dispatch.DropoffStop);

        Assert.False(dispatch.IsSigned);
        Assert.Single(dispatch.Vehicles);

        var publishedEvent = Assert.Single(publishedEvents);
        Assert.Equal(publishedEvent.DispatchId, dispatch.DispatchId);
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Driver)]
    [InlineData(UserRole.SyncJob)]
    public async Task CreateAsync_UserIsNotAOwner_ThrowsUnauthorizedAccessException(UserRole role)
    {
        // Setup
        var request = MakeRequest(carrierCompanyId);
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());
        var publishedEvents = new List<DispatchWriterEvent>();
        mockPublisher.Setup(p => p.Publish(It.IsAny<DispatchWriterEvent>()))
                    .Callback<DispatchWriterEvent>(e => publishedEvents.Add(e))
                    .Returns(Task.CompletedTask);
        // Create an instance of local Db context 
        using var db = InMemoryDbContextFactory.Create();
        // Add the unauthorized role
        await db.SeedUserAsync(shipperOwnerId, shipperCompanyId, role);
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var service = CreateServiceForShipper(db);
        db.ChangeTracker.Clear();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.CreateAsync(request));
        Assert.Empty(db.Dispatches);
        Assert.Empty(publishedEvents);
    }

    [Fact]
    public async Task CreateAsync_CompanyTypeIsNotCarrier_ThrowsValidationException()
    {
        // Setup
        var request = MakeRequest(shipperCompanyId);
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());
        var publishedEvents = new List<DispatchWriterEvent>();
        mockPublisher.Setup(p => p.Publish(It.IsAny<DispatchWriterEvent>()))
                    .Callback<DispatchWriterEvent>(e => publishedEvents.Add(e))
                    .Returns(Task.CompletedTask);
        // Create an instance of local Db context 
        using var db = InMemoryDbContextFactory.Create();
        // Add the unauthorized role
        await db.SeedUserAsync(shipperOwnerId, shipperCompanyId, UserRole.Owner);
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var service = CreateServiceForShipper(db);
        db.ChangeTracker.Clear();

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(request));
        Assert.Empty(db.Dispatches);
        Assert.Empty(publishedEvents);
    }

    [Fact]
    public async Task CreateAsync_OwnerOfCarrierCompany_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = MakeRequest(carrierCompanyId);
        mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());
        var publishedEvents = new List<DispatchWriterEvent>();
        mockPublisher.Setup(p => p.Publish(It.IsAny<DispatchWriterEvent>()))
                    .Callback<DispatchWriterEvent>(e => publishedEvents.Add(e))
                    .Returns(Task.CompletedTask);

        using var db = InMemoryDbContextFactory.Create();
        // A real Owner, but of the CARRIER company
        await db.SeedUserAsync(carrierOwnerId, carrierCompanyId, UserRole.Owner);
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        // Current user = carrierOwnerId
        var service = CreateServiceForCarrier(db);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.CreateAsync(request));
        Assert.Empty(db.Dispatches);
        Assert.Empty(publishedEvents);
    }

    [Theory]
    [InlineData(CompanyType.Shipper)]
    [InlineData(CompanyType.Carrier)]
    public async Task GetById_EmptyRequestId_ShouldThrowValidationException(CompanyType companyType)
    {
        using var db = InMemoryDbContextFactory.Create();

        var service = companyType == CompanyType.Shipper
            ? CreateServiceForShipper(db)
            : CreateServiceForCarrier(db);

        await Assert.ThrowsAsync<ValidationException>(() => service.GetByIdAsync(Guid.Empty));
    }

    [Theory]
    [InlineData(CompanyType.Shipper)]
    [InlineData(CompanyType.Carrier)]
    public async Task GetById_UnknownRequestId_ShouldThrowKeyNotFoundException(CompanyType companyType)
    {
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);

        var service = companyType == CompanyType.Shipper
            ? CreateServiceForShipper(db)
            : CreateServiceForCarrier(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByIdAsync(Guid.NewGuid()));
    }

    [Theory]
    [InlineData(CompanyType.Shipper)]
    [InlineData(CompanyType.Carrier)]
    public async Task GetById_ValidRequest_ShouldReturnDispatch(CompanyType companyType)
    {
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var serivce = companyType == CompanyType.Shipper
            ? CreateServiceForShipper(db)
            : CreateServiceForCarrier(db);

        var testedDispatch = await serivce.GetByIdAsync(seededDispatch.DispatchId);

        Assert.Equal(seededDispatch.DispatchId, testedDispatch.DispatchId);

        AssertCompanyMatches(seededDispatch.Carrier, testedDispatch.Carrier);
        AssertCompanyMatches(seededDispatch.Shipper, testedDispatch.Shipper);

        AssertStopMatches(seededDispatch.PickupStop, testedDispatch.PickupStop);
        AssertStopMatches(seededDispatch.DropoffStop, testedDispatch.DropoffStop);

        AssertVehiclesMatch(seededDispatch.Vehicles, testedDispatch.Vehicles);
        AssertDriversMatch(seededDispatch.Drivers, testedDispatch.Drivers);
    }

    [Theory]
    [InlineData(CompanyType.Shipper)]
    [InlineData(CompanyType.Carrier)]
    public async Task GetById_MultipleDispatchesValidRequest_ShouldReturnRequestedDispatch(CompanyType companyType)
    {
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch1 = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch2 = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);

        db.ChangeTracker.Clear();

        var serivce = companyType == CompanyType.Shipper
            ? CreateServiceForShipper(db)
            : CreateServiceForCarrier(db);

        var testedDispatch = await serivce.GetByIdAsync(seededDispatch2.DispatchId);

        Assert.Equal(seededDispatch2.DispatchId, testedDispatch.DispatchId);
    }

    [Theory]
    [InlineData(CompanyType.Shipper)]
    [InlineData(CompanyType.Carrier)]
    public async Task GetBatch_EmptyRequestIds_ShouldThrowValidationException(CompanyType companyType)
    {
        List<Guid> requestedIds = new();
        var request = new GetDispatchBatchRequest(requestedIds);
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(FailedValidationResult);

        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var serivce = companyType == CompanyType.Shipper
            ? CreateServiceForShipper(db)
            : CreateServiceForCarrier(db);

        await Assert.ThrowsAsync<ValidationException>(() => serivce.GetBatchAsync(request));
    }

    [Fact]
    public async Task GetBatch_ValidRequestIdsAllFound_ShouldReturnAllInFoundAndNoneInNotFound()
    {
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var dispatch1 = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        var dispatch2 = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);

        // Exist in the database but not requested
        await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var request = new GetDispatchBatchRequest(new List<Guid> { dispatch1.DispatchId, dispatch2.DispatchId });
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);
        var response = await service.GetBatchAsync(request);

        Assert.Equal(2, response.Found.ToList().Count);
        Assert.Contains(response.Found, d => d.DispatchId == dispatch1.DispatchId);
        Assert.Contains(response.Found, d => d.DispatchId == dispatch2.DispatchId);
        Assert.Empty(response.NotFound);
    }

    [Fact]
    public async Task GetBatch_NoRequestedIdsExist_ReturnsAllInNotFound()
    {
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        // Exist in the database but not requested
        await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var requestedIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var request = new GetDispatchBatchRequest(requestedIds);
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);
        var response = await service.GetBatchAsync(request);

        Assert.Empty(response.Found);
        Assert.Equal(requestedIds.Order(), response.NotFound.Order());
    }

    [Fact]
    public async Task GetBatch_SomeRequestedIdsExist_SplitsFoundAndNotFound()
    {
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seeded = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var unknownId = Guid.NewGuid();
        var request = new GetDispatchBatchRequest(new List<Guid> { seeded.DispatchId, unknownId });
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);
        var response = await service.GetBatchAsync(request);

        var found = Assert.Single(response.Found);
        Assert.Equal(seeded.DispatchId, found.DispatchId);
        var notFound = Assert.Single(response.NotFound);
        Assert.Equal(unknownId, notFound);
    }

    [Fact]
    public async Task GetBatch_ShipperUser_SeesCarrierCompany()
    {
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seeded = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var request = new GetDispatchBatchRequest(new List<Guid> { seeded.DispatchId });
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);
        var response = await service.GetBatchAsync(request);

        var found = Assert.Single(response.Found);
        AssertCompanyMatches(seeded.Carrier, found.Carrier);
    }

    [Fact]
    public async Task GetBatch_CarrierUser_SeesShipperCompany()
    {
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seeded = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var request = new GetDispatchBatchRequest(new List<Guid> { seeded.DispatchId });
        mockBatchValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForCarrier(db);
        var response = await service.GetBatchAsync(request);

        var found = Assert.Single(response.Found);
        AssertCompanyMatches(seeded.Shipper, found.Shipper);
    }

    [Fact]
    public async Task GetPaged_InvalidRequest_ThrowsValidationException()
    {
        var request = new GetDispatchesPagedRequest("abc-not-valid-guid", 500);

        mockPagedValidator
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(FailedValidationResult);

        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var serivce = CreateServiceForShipper(db);

        await Assert.ThrowsAsync<ValidationException>(() => serivce.GetPagedAsync(request));
    }

    [Theory]
    [InlineData(3, 2, 2, true)]
    [InlineData(2, 2, 2, false)]
    [InlineData(1, 2, 1, false)]
    [InlineData(0, 2, 0, false)]
    [Trait("Test for the \"HasMore\" logic", "Logic")]
    public async Task GetPaged_FirstPage_ReturnsUpToLimitAndCursorOnlyWhenMoreRemain(int seedCount, int limit, int expectedCount, bool expectCursor)
    {
        // Arrange
        var request = new GetDispatchesPagedRequest(null, limit);

        mockPagedValidator
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(SuccessfulValidationResult);

        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);

        int start = 0;
        while (start < seedCount)
        {
            await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
            ++start;
        }
        db.ChangeTracker.Clear();

        // Act 
        var serivce = CreateServiceForShipper(db);
        var response = await serivce.GetPagedAsync(request);

        var items = response.Items.ToList();
        Assert.Equal(expectedCount, items.Count);
        if (expectCursor)
        {
            Assert.Equal(items.Last().DispatchId.ToString(), response.Cursor);
        }
        else
        {
            Assert.Null(response.Cursor);
        }
        Assert.Equal(items.OrderBy(i => i.DispatchId), items);
    }


    [Fact]
    public async Task GetPaged_FollowingCursorToEnd_ReturnsEveryDispatchExactlyOnce()
    {
        // Arrange
        int limit = 2;
        int seededCount = 5;
        string? cursor = null;
        int pages = 0;

        mockPagedValidator
            .Setup(v => v.ValidateAsync(It.IsAny<GetDispatchesPagedRequest>(), default))
            .ReturnsAsync(SuccessfulValidationResult);

        using var db = InMemoryDbContextFactory.Create();
        List<Guid> seededIds = new();

        int start = 0;
        while (start < seededCount)
        {
            ++start;
            var dispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
            seededIds.Add(dispatch.DispatchId);
        }
        db.ChangeTracker.Clear();
        var service = CreateServiceForShipper(db);

        // Act
        List<Guid> returnedIds = new();
        do
        {
            var request = new GetDispatchesPagedRequest(cursor, limit);
            var response = await service.GetPagedAsync(request);

            returnedIds.AddRange(response.Items.Select(item => item.DispatchId));

            cursor = response.Cursor;
            pages++;
        } while (cursor is not null && pages < 10);

        // Assert
        Assert.Null(cursor);
        Assert.Equal(3, pages);
        Assert.Equal(seededIds.Order(), returnedIds.Order());
    }

    [Fact]
    public async Task GetPaged_MapsDispatchFieldsAndVehicleVins_ReturnSingleDispatch()
    {
        // Arrange
        var request = new GetDispatchesPagedRequest(null, 500);

        mockPagedValidator
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(SuccessfulValidationResult);

        using var db = InMemoryDbContextFactory.Create();
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var service = CreateServiceForShipper(db);

        // Act
        var response = await service.GetPagedAsync(request);

        // Assert
        var item = Assert.Single(response.Items);
        Assert.Equal(seededDispatch.DispatchId, item.DispatchId);
        Assert.Equal(seededDispatch.CarrierId, item.CarrierId);
        Assert.Equal(seededDispatch.ShipperId, item.ShipperId);
        Assert.Equal(seededDispatch.Price, item.PriceTotal);
        Assert.Equal(seededDispatch.PickupDate, item.PickupDate);
        Assert.Equal(seededDispatch.DropoffDate, item.DropoffDate);
        Assert.Equal(seededDispatch.DispatchStatus, item.DispatchStatus);
        Assert.Equal(seededDispatch.CreatedAt, item.CreatedAt);
        Assert.Equal(
            seededDispatch.Vehicles.Select(v => v.Vin).Order(),
            item.Vehicles.Select(v => v.Vin).Order());
    }

    // ----- Assign driver -----

    [Fact]
    public async Task AssignDriver_UnknownDispatch_ThrowsKeyNotFoundException()
    {
        using var db = InMemoryDbContextFactory.Create();
        var service = CreateServiceForCarrier(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.AssignDriverAsync(Guid.NewGuid(), new AssignDriverRequest(Guid.NewGuid())));
    }

    [Fact]
    public async Task AssignDriver_UnknownDriverId_ThrowsArgumentException_NothingSaved()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var service = CreateServiceForCarrier(db);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.AssignDriverAsync(seededDispatch.DispatchId, new AssignDriverRequest(Guid.NewGuid())));

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(DispatchStatus.NotSigned, reloaded.DispatchStatus);
        Assert.Empty(reloaded.Drivers);
    }

    [Theory]
    [InlineData(UserRole.Driver, false, true)]  // inactive driver
    [InlineData(UserRole.Driver, true, false)]  // driver of another company
    [InlineData(UserRole.Owner, true, true)]    // active user who is not a driver
    public async Task AssignDriver_UserIsNotAnActiveDriverOfTheCompany_ThrowsArgumentException_NothingSaved(
        UserRole role, bool isActive, bool inCarrierCompany)
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        var userId = Guid.NewGuid();
        await db.SeedUserAsync(userId, inCarrierCompany ? carrierCompanyId : shipperCompanyId, role, isActive);
        db.ChangeTracker.Clear();

        var service = CreateServiceForCarrier(db);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.AssignDriverAsync(seededDispatch.DispatchId, new AssignDriverRequest(userId)));

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(DispatchStatus.NotSigned, reloaded.DispatchStatus);
        Assert.Empty(reloaded.Drivers);
    }

    [Fact]
    public async Task AssignDriver_ActiveDriver_AssignsDriverAndSetsPendingDelivery()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        var driverId = Guid.NewGuid();
        await db.SeedUserAsync(driverId, carrierCompanyId, UserRole.Driver);
        db.ChangeTracker.Clear();

        var service = CreateServiceForCarrier(db);

        // Act
        await service.AssignDriverAsync(seededDispatch.DispatchId, new AssignDriverRequest(driverId));

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(DispatchStatus.PendingDelivery, reloaded.DispatchStatus);
        var assigned = Assert.Single(reloaded.Drivers);
        Assert.Equal(driverId, assigned.DriverId);
    }

    [Fact]
    public async Task AssignDriver_DispatchAlreadyHasDriver_ReplacesDriver()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        var oldDriverId = Guid.NewGuid();
        var newDriverId = Guid.NewGuid();
        await db.SeedUserAsync(oldDriverId, carrierCompanyId, UserRole.Driver);
        await db.SeedUserAsync(newDriverId, carrierCompanyId, UserRole.Driver);
        await db.SeedDispatchDriverAsync(seededDispatch.DispatchId, oldDriverId);
        db.ChangeTracker.Clear();

        var service = CreateServiceForCarrier(db);

        // Act
        await service.AssignDriverAsync(seededDispatch.DispatchId, new AssignDriverRequest(newDriverId));

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        var assigned = Assert.Single(reloaded.Drivers);
        Assert.Equal(newDriverId, assigned.DriverId);
    }

    [Fact]
    public async Task AssignDriver_SameDriverAgain_DoesNotDuplicate()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        var driverId = Guid.NewGuid();
        await db.SeedUserAsync(driverId, carrierCompanyId, UserRole.Driver);
        await db.SeedDispatchDriverAsync(seededDispatch.DispatchId, driverId);
        db.ChangeTracker.Clear();

        var service = CreateServiceForCarrier(db);

        // Act
        await service.AssignDriverAsync(seededDispatch.DispatchId, new AssignDriverRequest(driverId));

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        var assigned = Assert.Single(reloaded.Drivers);
        Assert.Equal(driverId, assigned.DriverId);
    }

    [Fact]
    public async Task AssignDriver_NullDriverId_UnassignsDriver()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        var driverId = Guid.NewGuid();
        await db.SeedUserAsync(driverId, carrierCompanyId, UserRole.Driver);
        await db.SeedDispatchDriverAsync(seededDispatch.DispatchId, driverId);
        db.ChangeTracker.Clear();

        var service = CreateServiceForCarrier(db);

        // Act
        await service.AssignDriverAsync(seededDispatch.DispatchId, new AssignDriverRequest(null));

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Empty(reloaded.Drivers);
    }

    // ----- Delete -----

    [Fact]
    public async Task Delete_UnknownDispatch_ThrowsKeyNotFoundException_NoEventPublished()
    {
        using var db = InMemoryDbContextFactory.Create();
        var service = CreateServiceForShipper(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteAsync(Guid.NewGuid()));

        mockPublisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_DeliveredDispatch_ThrowsValidationException_NothingChanged()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId, DispatchStatus.Delivered);
        db.ChangeTracker.Clear();

        var service = CreateServiceForShipper(db);

        // Act
        await Assert.ThrowsAsync<ValidationException>(() => service.DeleteAsync(seededDispatch.DispatchId));

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(DispatchStatus.Delivered, reloaded.DispatchStatus);
        Assert.Single(reloaded.Vehicles);
        Assert.NotNull(reloaded.PickupStop);
        Assert.NotNull(reloaded.DropoffStop);
        mockPublisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_ValidDispatch_CancelsRemovesChildrenAndPublishesDeleteEvent()
    {
        // Arrange
        var publishedEvents = new List<DispatchDeleteEvent>();
        mockPublisher.Setup(p => p.Publish(It.IsAny<DispatchDeleteEvent>()))
                    .Callback<DispatchDeleteEvent>(e => publishedEvents.Add(e))
                    .Returns(Task.CompletedTask);

        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        var driverId = Guid.NewGuid();
        await db.SeedUserAsync(driverId, carrierCompanyId, UserRole.Driver);
        await db.SeedDispatchDriverAsync(seededDispatch.DispatchId, driverId);
        db.ChangeTracker.Clear();

        var service = CreateServiceForShipper(db);

        // Act
        await service.DeleteAsync(seededDispatch.DispatchId);

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(DispatchStatus.Canceled, reloaded.DispatchStatus);
        Assert.Empty(reloaded.Vehicles);
        Assert.Empty(reloaded.Drivers);
        Assert.Null(reloaded.PickupStop);
        Assert.Null(reloaded.DropoffStop);
        Assert.Empty(db.Stops);

        var publishedEvent = Assert.Single(publishedEvents);
        Assert.Equal(EventType.Delete, publishedEvent.Type);
        Assert.Equal(seededDispatch.DispatchId, publishedEvent.DispatchId);
    }

    // ----- Update -----

    [Fact]
    public async Task Update_InvalidRequest_ThrowsValidationException_NothingSaved()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var request = MakeUpdateRequest([]);
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(FailedValidationResult());

        var service = CreateServiceForShipper(db);

        // Act
        await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(seededDispatch.DispatchId, request));

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(seededDispatch.Price, reloaded.Price);
        Assert.Single(reloaded.Vehicles);
        mockPublisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_UnknownDispatch_ThrowsKeyNotFoundException()
    {
        var request = MakeUpdateRequest([]);
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        using var db = InMemoryDbContextFactory.Create();
        var service = CreateServiceForShipper(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), request));

        mockPublisher.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(DispatchStatus.Canceled)]
    [InlineData(DispatchStatus.Delivered)]
    [InlineData(DispatchStatus.PendingDelivery)]
    public async Task Update_DispatchInLockedStatus_ThrowsValidationException_NothingSaved(DispatchStatus status)
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId, status);
        db.ChangeTracker.Clear();

        var request = MakeUpdateRequest([]);
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);

        // Act
        await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(seededDispatch.DispatchId, request));

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(status, reloaded.DispatchStatus);
        Assert.Equal(seededDispatch.Price, reloaded.Price);
        mockPublisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_ExistingVehicleOfThisDispatch_UpdatesVehicleInPlace()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var vehicleId = seededDispatch.Vehicles.Single().VehicleId;
        var request = MakeUpdateRequest([
            new UpdateVehicleRequest(vehicleId, "9HGCM82633AUPDATE", 2019, "Ford", "Fusion", "Red")
        ]);
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);

        // Act
        await service.UpdateAsync(seededDispatch.DispatchId, request);

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        var vehicle = Assert.Single(reloaded.Vehicles);
        Assert.Equal(vehicleId, vehicle.VehicleId);
        Assert.Equal("9HGCM82633AUPDATE", vehicle.Vin);
        Assert.Equal(2019, vehicle.Year);
        Assert.Equal("Ford", vehicle.Make);
        Assert.Equal("Fusion", vehicle.Model);
        Assert.Equal("Red", vehicle.Color);
    }

    [Fact]
    public async Task Update_VehicleOfAnotherDispatch_ThrowsArgumentException_NothingSaved()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var dispatchA = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        var dispatchB = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var otherDispatchVehicleId = dispatchB.Vehicles.Single().VehicleId;
        var request = MakeUpdateRequest([
            new UpdateVehicleRequest(otherDispatchVehicleId, null, 2021, "Ford", "Fusion", null)
        ]);
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(dispatchA.DispatchId, request));

        // Assert
        var reloadedA = await ReloadDispatchAsync(db, dispatchA.DispatchId);
        Assert.Equal(dispatchA.Price, reloadedA.Price);
        Assert.Single(reloadedA.Vehicles);

        var reloadedB = await ReloadDispatchAsync(db, dispatchB.DispatchId);
        var vehicleB = Assert.Single(reloadedB.Vehicles);
        Assert.Equal("Honda", vehicleB.Make);
        mockPublisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_NewVehicleId_AddsVehicleWithThatId()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var existingVehicleId = seededDispatch.Vehicles.Single().VehicleId;
        var newVehicleId = Guid.NewGuid();
        var request = MakeUpdateRequest([
            new UpdateVehicleRequest(existingVehicleId, null, 2020, "Honda", "Accord", null),
            new UpdateVehicleRequest(newVehicleId, "1HGCM82633ANEW1", 2022, "Toyota", "Camry", "Silver")
        ]);
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);

        // Act
        await service.UpdateAsync(seededDispatch.DispatchId, request);

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(2, reloaded.Vehicles.Count);
        var created = Assert.Single(reloaded.Vehicles, v => v.VehicleId == newVehicleId);
        Assert.Equal("1HGCM82633ANEW1", created.Vin);
        Assert.Equal(2022, created.Year);
        Assert.Equal("Toyota", created.Make);
        Assert.Equal("Camry", created.Model);
        Assert.Equal("Silver", created.Color);
        Assert.Equal(reloaded.PickupStopId, created.PickupStopId);
        Assert.Equal(reloaded.DropoffStopId, created.DropoffStopId);
    }

    [Fact]
    public async Task Update_VehicleMissingFromRequest_IsRemovedFromDatabase()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var originalVehicleId = seededDispatch.Vehicles.Single().VehicleId;
        var newVehicleId = Guid.NewGuid();
        var request = MakeUpdateRequest([
            new UpdateVehicleRequest(newVehicleId, null, 2021, "Nissan", "Altima", null)
        ]);
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);

        // Act
        await service.UpdateAsync(seededDispatch.DispatchId, request);

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        var remaining = Assert.Single(reloaded.Vehicles);
        Assert.Equal(newVehicleId, remaining.VehicleId);
        Assert.Null(await db.Vehicles.FindAsync(originalVehicleId));
    }

    [Fact]
    public async Task Update_ValidRequest_UpdatesDispatchStopsAndReturnsResponse()
    {
        // Arrange
        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var vehicleId = seededDispatch.Vehicles.Single().VehicleId;
        var newPickupDate = DateTime.UtcNow.AddDays(3);
        var newDropoffDate = DateTime.UtcNow.AddDays(4);
        var request = new UpdateDispatchRequest(999m, newPickupDate, newDropoffDate, "New description",
            pickupStopRequest, dropoffStopRequest,
            [new UpdateVehicleRequest(vehicleId, null, 2020, "Honda", "Accord", null)]);
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);

        // Act
        var response = await service.UpdateAsync(seededDispatch.DispatchId, request);

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(999m, reloaded.Price);
        Assert.Equal(newPickupDate, reloaded.PickupDate);
        Assert.Equal(newDropoffDate, reloaded.DropoffDate);
        Assert.Equal("New description", reloaded.Description);
        AssertStopMatchesRequest(pickupStopRequest, reloaded.PickupStop);
        AssertStopMatchesRequest(dropoffStopRequest, reloaded.DropoffStop);

        Assert.Equal(seededDispatch.DispatchId, response.DispatchId);
        Assert.Equal(999m, response.Price);
        Assert.Equal(newPickupDate, response.PickupDate);
        Assert.Equal(newDropoffDate, response.DropoffDate);
        Assert.Equal("New description", response.Description);
    }

    [Fact]
    public async Task Update_ValidRequest_PublishesUpdateEvent()
    {
        // Arrange
        var publishedEvents = new List<DispatchUpdateEvent>();
        mockPublisher.Setup(p => p.Publish(It.IsAny<DispatchUpdateEvent>()))
                    .Callback<DispatchUpdateEvent>(e => publishedEvents.Add(e))
                    .Returns(Task.CompletedTask);

        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var vehicleId = seededDispatch.Vehicles.Single().VehicleId;
        var request = MakeUpdateRequest([
            new UpdateVehicleRequest(vehicleId, null, 2020, "Honda", "Accord", null)
        ]);
        mockUpdateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(SuccessfulValidationResult());

        var service = CreateServiceForShipper(db);

        // Act
        await service.UpdateAsync(seededDispatch.DispatchId, request);

        // Assert
        var publishedEvent = Assert.Single(publishedEvents);
        Assert.Equal(EventType.Update, publishedEvent.Type);
        Assert.Equal(seededDispatch.DispatchId, publishedEvent.DispatchId);
        Assert.Equal(shipperCompanyId, publishedEvent.ShipperId);
        Assert.Equal(carrierCompanyId, publishedEvent.CarrierId);
        Assert.Equal(request.Price, publishedEvent.PriceTotal);
        Assert.Equal(DispatchStatus.NotSigned, publishedEvent.DispatchStatus);
    }

    // ----- Accept -----

    [Fact]
    public async Task Accept_UnknownDispatch_ThrowsKeyNotFoundException_NoEventPublished()
    {
        using var db = InMemoryDbContextFactory.Create();
        var service = CreateServiceForCarrier(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AcceptDispatch(Guid.NewGuid()));

        mockPublisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Accept_ValidDispatch_SetsPendingPickupAndPublishesUpdateEvent()
    {
        // Arrange
        var publishedEvents = new List<DispatchUpdateEvent>();
        mockPublisher.Setup(p => p.Publish(It.IsAny<DispatchUpdateEvent>()))
                    .Callback<DispatchUpdateEvent>(e => publishedEvents.Add(e))
                    .Returns(Task.CompletedTask);

        using var db = InMemoryDbContextFactory.Create();
        await db.SeedCompaniesAsync(shipperCompanyId, carrierCompanyId);
        var seededDispatch = await db.SeedDispatchAsync(shipperCompanyId, carrierCompanyId);
        db.ChangeTracker.Clear();

        var service = CreateServiceForCarrier(db);

        // Act
        await service.AcceptDispatch(seededDispatch.DispatchId);

        // Assert
        var reloaded = await ReloadDispatchAsync(db, seededDispatch.DispatchId);
        Assert.Equal(DispatchStatus.PendingPickup, reloaded.DispatchStatus);
        Assert.All(reloaded.Vehicles, v => Assert.Equal(VehicleStatus.PreparingForShipment, v.VehicleStatus));

        var publishedEvent = Assert.Single(publishedEvents);
        Assert.Equal(EventType.Update, publishedEvent.Type);
        Assert.Equal(seededDispatch.DispatchId, publishedEvent.DispatchId);
        Assert.Equal(DispatchStatus.PendingPickup, publishedEvent.DispatchStatus);
    }
}
