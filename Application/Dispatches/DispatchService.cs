using Application.Events;
using Application.Interfaces;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Dispatches;

public class DispatchService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<DispatchService> _logger;
    private readonly IValidator<CreateDispatchRequest> _createValidator;
    private readonly IValidator<GetDispatchBatchRequest> _batchValidator;
    private readonly IValidator<AssignDriverRequest> _assignDriverValidator;
    private readonly IValidator<UpdateDispatchRequest> _updateValidator;
    private readonly IValidator<GetDispatchesPagedRequest> _pagedValidator;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICurrentUserService _currentUser;

    public DispatchService(
        IApplicationDbContext db,
        ILogger<DispatchService> logger,
        IValidator<CreateDispatchRequest> createValidator,
        IValidator<GetDispatchBatchRequest> batchValidator,
        IValidator<AssignDriverRequest> assignDriverValidator,
        IValidator<UpdateDispatchRequest> updateValidator,
        IValidator<GetDispatchesPagedRequest> pagedValidator,
        IEventPublisher eventPublisher,
        ICurrentUserService currentUser)
    {
        _db = db;
        _logger = logger;
        _createValidator = createValidator;
        _batchValidator = batchValidator;
        _assignDriverValidator = assignDriverValidator;
        _updateValidator = updateValidator;
        _pagedValidator = pagedValidator;
        _eventPublisher = eventPublisher;
        _currentUser = currentUser;
    }

    // ----- Create -----
    public async Task<CreateDispatchResponse> CreateAsync(CreateDispatchRequest request)
    {
        var result = await _createValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        // Assign the company id of the current user who role is owner 
        var ownerCompanyId = await _db.Users
                        .Where(c => c.UserRole.Equals(UserRole.Owner) && c.UserId == _currentUser.UserId)
                        .Select(u => u.CompanyId)
                        .SingleOrDefaultAsync();

        // Throw error let the error to appear when unit test
        // Old code before writing unit test
        // var shipperCompany = await _db.Companies.FirstOrDefaultAsync(c => c.CompanyId.Equals(ownerCompanyId))
        //     ?? throw new KeyNotFoundException($"Owner company {ownerCompanyId} not found");
        var shipperCompany = await _db.Companies
            .FirstOrDefaultAsync(c => c.CompanyId.Equals(ownerCompanyId)
            && c.CompanyType == CompanyType.Shipper)
              ?? throw new UnauthorizedAccessException($"Current user with id {_currentUser.UserId} is not an owner");

        // Throw error let the error to appear when unit test
        var carrierCompany = await _db.Companies
            .FirstOrDefaultAsync(c => c.CompanyId.Equals(request.CarrierId)
            && c.CompanyType == CompanyType.Carrier)
            ?? throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.CarrierId), $"{request.CarrierId} is not a carrier company")
            });

        var dispatch = DispatchMapper.ToDomain(request, ownerCompanyId);

        dispatch.SetShipperCompany(shipperCompany);

        dispatch.SetCarrierCompany(carrierCompany);

        // Publish message to an existing topic running in a LocalStack container
        await _eventPublisher.Publish(new DispatchWriterEvent(
            EventType.Create,
            dispatch.DispatchId,
            dispatch.ShipperId,
            dispatch.CarrierId,
            dispatch.Price,
            dispatch.PickupDate,
            dispatch.DropoffDate,
            dispatch.DispatchStatus,
            dispatch.Vehicles.Select(v => new DispatchWriterVehicle(v.Vin)),
            dispatch.CreatedAt));

        _db.Dispatches.Add(dispatch);
        await _db.SaveChangesAsync();

        return DispatchMapper.ToResponse(dispatch);
    }

    // ----- Get by id -----
    public async Task<DispatchResponse> GetByIdAsync(Guid dispatchId)
    {
        if (dispatchId == Guid.Empty)
            throw new ValidationException(new[] { new ValidationFailure(nameof(dispatchId), "The requested id must not be empty") });

        var dispatch = await _db.Dispatches
                .Include(d => d.PickupStop)
                .Include(d => d.DropoffStop)
                .Include(d => d.Carrier)
                .Include(d => d.Shipper)
                .Include(d => d.Vehicles).ThenInclude(v => v.PickupStop)
                .Include(d => d.Vehicles).ThenInclude(v => v.DropoffStop)
                .Include(d => d.Drivers).ThenInclude(dd => dd.Driver)
            .FirstOrDefaultAsync(d => d.DispatchId == dispatchId);


        if (dispatch is null)
            throw new KeyNotFoundException($"Dispatch {dispatchId} not found.");

        return DispatchMapper.ToDispatchResponse(dispatch);
    }

    // ----- Get batch -----
    public async Task<GetDispatchBatchResponse> GetBatchAsync(GetDispatchBatchRequest request)
    {
        var result = await _batchValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var requestedIds = request.DispatchIds.ToList();
        // Before writing unit test, miss the case the the requestedId list is empty. Bad code


        var query = _db.Dispatches
            .Include(d => d.PickupStop)
            .Include(d => d.DropoffStop)
            .Include(d => d.Vehicles).ThenInclude(v => v.PickupStop)
            .Include(d => d.Vehicles).ThenInclude(v => v.DropoffStop)
            .Include(d => d.Drivers).ThenInclude(dd => dd.Driver)
            .Where(d => requestedIds.Contains(d.DispatchId));

        // Check what type of company the current user is 
        // and return the opposite company type data
        if (_currentUser.CompanyType == CompanyType.Carrier)
        {
            query = query.Include(d => d.Shipper);
        }
        else if (_currentUser.CompanyType == CompanyType.Shipper)
        {
            query = query.Include(d => d.Carrier);
        }

        var dispatches = await query.ToListAsync();

        var foundIds = dispatches.Select(d => d.DispatchId).ToHashSet();
        var notFound = requestedIds.Where(id => !foundIds.Contains(id));

        return new GetDispatchBatchResponse(
            dispatches.Count == 0 ? [] : dispatches.Select(x => DispatchMapper.ToDispatchResponse(x)),
            notFound);
    }
    /// <summary>
    /// Function used to sync data with OpenSearch. OpenSearch Dispatch only need Vehicles information
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    // ----- Get paged -----
    public async Task<PageResponseWithCursor<DispatchWriterDto>> GetPagedAsync(GetDispatchesPagedRequest request)
    {
        var result = await _pagedValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
        
        //   - parse request.Cursor as a Guid (treat null/empty as "start from the beginning")
        Guid? cursor = string.IsNullOrEmpty(request.Cursor) ? null : Guid.Parse(request.Cursor);

        IQueryable<Dispatch> query = _db.Dispatches.Include(d => d.Vehicles);

        if (cursor.HasValue)
            query = query.Where(d => d.DispatchId > cursor.Value);

        //   - order by DispatchId ascending (stable, unique order — required for keyset pagination)
        query = query
            .OrderBy(d => d.DispatchId)
            .Take(request.Limit + 1);

        var dispatches = await query.ToListAsync();

        // Check has more to avoid the case where dispatched.Count = 500 and request.Limit = 500. 
        // Then a second query is sent to the database
        // This code to avoid that 
        var hasMore = dispatches.Count > request.Limit;
        if (hasMore)
            dispatches.RemoveAt(dispatches.Count - 1);

        //   - map each Dispatch to a DispatchWriterDto
        var items = dispatches.Select(d => new DispatchWriterDto(
            d.DispatchId,
            d.CarrierId,
            d.ShipperId,
            d.Price,
            d.PickupDate,
            d.DropoffDate,
            d.DispatchStatus,
            d.Vehicles.Select(v => new DispatchWriterVehicle(v.Vin)),
            d.CreatedAt));

        // If there are still dispatch in the database, return the id of the last dispatch to use 
        // as cursor for the next call to this endpoint, if 
        return new PageResponseWithCursor<DispatchWriterDto>(items,
             hasMore ? dispatches.Last().DispatchId.ToString() : null);
    }

    // ----- Assign driver -----
    public async Task AssignDriverAsync(Guid dispatchId, AssignDriverRequest request)
    {
        var dispatch = await _db.Dispatches
            .Include(d => d.Drivers)
            .FirstOrDefaultAsync(d => d.DispatchId == dispatchId);

        if (dispatch is null)
            throw new KeyNotFoundException($"Dispatch {dispatchId} not found.");

        dispatch.UpdateStatus(DispatchStatus.PendingDelivery);

        if (request.DriverId is null)
        {
            dispatch.Drivers.Clear();
        }
        else
        {
            var driver = await _db.Users
                .FirstOrDefaultAsync(
                    u => u.CompanyId == _currentUser.CompanyId
                    &&
                    u.UserRole == UserRole.Driver
                    &&
                    u.UserId == request.DriverId);

            if (driver is null || !driver.IsActive)
                throw new ArgumentException("DriverId does not reference an active driver.", nameof(request.DriverId));

            if (dispatch.Drivers.Any())
            {
                dispatch.Drivers.Clear();
            }

            dispatch.Drivers.Add(new DispatchDriver
            {
                DispatchId = dispatch.DispatchId,
                DriverId = driver.UserId
            });
        }

        await _db.SaveChangesAsync();
    }

    // ----- Delete -----
    public async Task DeleteAsync(Guid dispatchId)
    {
        var dispatch = await _db.Dispatches
            .Include(d => d.Vehicles)
            .Include(d => d.Drivers)
            .Include(d => d.PickupStop)
            .Include(d => d.DropoffStop)
            .FirstOrDefaultAsync(d => d.DispatchId == dispatchId);

        if (dispatch is null)
            throw new KeyNotFoundException($"Dispatch {dispatchId} not found.");

        if (dispatch.DispatchStatus == DispatchStatus.Delivered)
            throw new ValidationException(new[] { new ValidationFailure(
                nameof(Dispatch.DispatchStatus), "Cannot delete a dispatch that has already been delivered.") });

        var pickupStop = dispatch.PickupStop;
        var dropoffStop = dispatch.DropoffStop;

        dispatch.Cancel();

        if (pickupStop is not null)
            _db.Stops.Remove(pickupStop);
        if (dropoffStop is not null)
            _db.Stops.Remove(dropoffStop);

        // Publish message to an existing topic running in a LocalStack container
        await _eventPublisher.Publish(new DispatchDeleteEvent(EventType.Delete, dispatch.DispatchId));

        await _db.SaveChangesAsync();
    }

    // ----- Update -----
    public async Task<DispatchResponse> UpdateAsync(Guid dispatchId, UpdateDispatchRequest request)
    {
        var result = await _updateValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var dispatch = await _db.Dispatches
            .Include(d => d.Vehicles)
            .Include(d => d.Carrier)
            .Include(d => d.Shipper)
            .Include(d => d.PickupStop)
            .Include(d => d.DropoffStop)
            .FirstOrDefaultAsync(d => d.DispatchId == dispatchId);

        if (dispatch is null)
            throw new KeyNotFoundException($"Dispatch {dispatchId} not found.");

        if (dispatch.DispatchStatus is DispatchStatus.Canceled or DispatchStatus.Delivered or DispatchStatus.PendingDelivery)
            throw new ValidationException(new[] { new ValidationFailure(
                nameof(Dispatch.DispatchStatus), $"Cannot update a dispatch with status {dispatch.DispatchStatus}.") });

        // List of all vehicle id in request
        var requestedVehicleIds = request.Vehicles
            .Select(v => v.VehicleId);

        // List of all vehicle id in current database
        var storedVehicleIds = _db.Vehicles.Select(v => v.VehicleId);

        // Update proceed in 3 steps: update first, delete second, and create third
        // Step 1: to update a vehicle, check if the vehicle ids of a dispatch contains 
        // the request vehicle ids, if yes then check if the request vehicle ids belong 
        // to another dispatch, if no then update, if yes throw error and stop this function
        foreach (var item in request.Vehicles.Where(v => storedVehicleIds.Contains(v.VehicleId)))
        {
            var vehicle = dispatch.Vehicles.FirstOrDefault(v => v.VehicleId == item.VehicleId);
            if (vehicle is null)
                throw new ArgumentException($"VehicleId {item.VehicleId} does not belong to this dispatch.");

            vehicle.UpdateDetails(item.Vin, item.Color, item.Year, item.Make, item.Model);
        }

        // Step 2: after update, proceed to delete any vehicle ids of a dispatch that are not 
        // sent in the request vehicle ids
        var vehiclesToRemove = dispatch.Vehicles.Where(v => !requestedVehicleIds.Contains(v.VehicleId)).ToList();
        foreach (var vehicle in vehiclesToRemove)
            _db.Vehicles.Remove(vehicle);


        // Step 3: finally for each request vehicle id that are not belong to this dispatch
        // and not belong to any other dispatch then proceed to create new vehicle 
        foreach (var item in request.Vehicles.Where(v => !storedVehicleIds.Contains(v.VehicleId)))
        {
            var newVehicle = Vehicle.CreateVehicle(dispatch, item.VehicleId, dispatch.PickupStop!, dispatch.DropoffStop!,
                item.Vin, item.Year!.Value, item.Make!, item.Model!, item.Color);
            _db.Vehicles.Add(newVehicle);
        }

        dispatch.PickupStop!.UpdateDetails(request.PickupStop.Address, request.PickupStop.LocationName,
            request.PickupStop.ContactName, request.PickupStop.ContactPhone, request.PickupStop.ContactEmail);
        dispatch.DropoffStop!.UpdateDetails(request.DropoffStop.Address, request.DropoffStop.LocationName,
            request.DropoffStop.ContactName, request.DropoffStop.ContactPhone, request.DropoffStop.ContactEmail);

        dispatch.UpdateDetails(request.Price, request.PickupDate, request.DropoffDate, request.Description);

        // Publish message to an existing topic running in a LocalStack container
        await _eventPublisher.Publish(new DispatchUpdateEvent(
            EventType.Update,
            dispatch.DispatchId,
            dispatch.ShipperId,
            dispatch.CarrierId,
            dispatch.Price,
            dispatch.PickupDate,
            dispatch.DropoffDate,
            dispatch.DispatchStatus,
            dispatch.Vehicles.Select(v => new DispatchUpdateVehicle(v.Vin)),
            dispatch.CreatedAt));

        await _db.SaveChangesAsync();

        return DispatchMapper.ToDispatchResponse(dispatch);
    }

    // ----------- Accept ------------------
    public async Task AcceptDispatch(Guid dispatchId)
    {
        var dispatch = await _db.Dispatches
            .Include(d => d.Vehicles)
            .FirstOrDefaultAsync(d => d.DispatchId.Equals(dispatchId));

        if (dispatch is null)
            throw new KeyNotFoundException($"Dispatch {dispatchId} not found.");

        dispatch.UpdateStatus(DispatchStatus.PendingPickup);

        await _eventPublisher.Publish(new DispatchUpdateEvent(
            EventType.Update,
            dispatch.DispatchId,
            dispatch.ShipperId,
            dispatch.CarrierId,
            dispatch.Price,
            dispatch.PickupDate,
            dispatch.DropoffDate,
            dispatch.DispatchStatus,
            dispatch.Vehicles.Select(v => new DispatchUpdateVehicle(v.Vin)),
            dispatch.CreatedAt));

        await _db.SaveChangesAsync();
    }
}