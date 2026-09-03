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

    public async Task<CreateDispatchResponse> CreateAsync(CreateDispatchRequest request)
    {
        var result = await _createValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var dispatch = DispatchMapper.ToDomain(request, _currentUser.UserId);

        // Publish message to an existing topic running in a LocalStack container
        await _eventPublisher.Publish(new DispatchWriterEvent(
            EventType.Create,
            dispatch.DispatchId,
            dispatch.Price,
            dispatch.PickupDate,
            dispatch.DropoffDate,
            dispatch.DispatchStatus,
            dispatch.Vehicles.Select(v => new DispatchWriterVehicle(v.Vin))));

        _db.Dispatches.Add(dispatch);
        await _db.SaveChangesAsync();

        return DispatchMapper.ToResponse(dispatch);
    }

    public async Task<DispatchResponse> GetByIdAsync(Guid dispatchId)
    {
        var dispatch = await _db.Dispatches
            .Include(d => d.PickupStop)
            .Include(d => d.DropoffStop)
            .Include(d => d.Vehicles).ThenInclude(v => v.PickupStop)
            .Include(d => d.Vehicles).ThenInclude(v => v.DropoffStop)
            .Include(d => d.Drivers).ThenInclude(dd => dd.Driver)
            .FirstOrDefaultAsync(d => d.DispatchId == dispatchId);

        if (dispatch is null)
            throw new KeyNotFoundException($"Dispatch {dispatchId} not found.");

        return DispatchMapper.ToDispatchResponse(dispatch);
    }

    public async Task<GetDispatchBatchResponse> GetBatchAsync(GetDispatchBatchRequest request)
    {
        var result = await _batchValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var requestedIds = request.DispatchIds.ToList();

        var dispatches = await _db.Dispatches
            .Include(d => d.PickupStop)
            .Include(d => d.DropoffStop)
            .Include(d => d.Vehicles).ThenInclude(v => v.PickupStop)
            .Include(d => d.Vehicles).ThenInclude(v => v.DropoffStop)
            .Include(d => d.Drivers).ThenInclude(dd => dd.Driver)
            .Where(d => requestedIds.Contains(d.DispatchId))
            .ToListAsync();

        var foundIds = dispatches.Select(d => d.DispatchId).ToHashSet();
        var notFound = requestedIds.Where(id => !foundIds.Contains(id));

        return new GetDispatchBatchResponse(
            dispatches.Select(DispatchMapper.ToDispatchResponse),
            notFound);
    }

    public async Task<PageResponseWithCursor<DispatchWriterDto>> GetPagedAsync(GetDispatchesPagedRequest request)
    {
        var result = await _pagedValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        //   - parse request.Cursor as a Guid (treat null/empty as "start from the beginning")
        Guid? cursor = string.IsNullOrEmpty(request.Cursor) ? null : Guid.Parse(request.Cursor);

        IQueryable<Dispatch> query = _db.Dispatches.Include(d => d.Vehicles);

        //   - filter: DispatchId > cursor
        if (cursor.HasValue)
            query = query.Where(d => d.DispatchId > cursor.Value);

        //   - order by DispatchId ascending (stable, unique order — required for keyset pagination)
        query = query
            .OrderBy(d => d.DispatchId)
            .Take(request.Limit);

        var dispatches = await query.ToListAsync();
        //   - map each Dispatch to a DispatchWriterDto
        var items = dispatches.Select(d => new DispatchWriterDto(d.DispatchId,
            d.Price,
            d.PickupDate,
            d.DropoffDate,
            d.DispatchStatus,
            d.Vehicles.Select(v => new DispatchWriterVehicle(v.Vin))));

        return new PageResponseWithCursor<DispatchWriterDto>(items,
            dispatches.Count == request.Limit ? dispatches.Last().DispatchId.ToString() : null);
    }

    public async Task AssignDriverAsync(Guid dispatchId, AssignDriverRequest request)
    {
        var result = await _assignDriverValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var dispatch = await _db.Dispatches
            .Include(d => d.Drivers)
            .FirstOrDefaultAsync(d => d.DispatchId == dispatchId);

        if (dispatch is null)
            throw new KeyNotFoundException($"Dispatch {dispatchId} not found.");

        if (dispatch.CarrierId == Guid.Empty)
            throw new ArgumentException("Dispatch does not have a carrier assigned.", nameof(dispatchId));

        dispatch.UpdateStatus(DispatchStatus.PendingDelivery);

        var driver = await _db.Users.FirstOrDefaultAsync(u => u.UserId == request.DriverId);
        if (driver is null || !driver.IsActive)
            throw new ArgumentException("DriverId does not reference an active driver.", nameof(request.DriverId));

        var alreadyAssigned = dispatch.Drivers.Any(dd => dd.DriverId == request.DriverId);
        if (alreadyAssigned)
            return;

        dispatch.Drivers.Add(new DispatchDriver
        {
            DispatchId = dispatch.DispatchId,
            DriverId = driver.UserId
        });
        await _db.SaveChangesAsync();
    }

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

    public async Task<DispatchResponse> UpdateAsync(Guid dispatchId, UpdateDispatchRequest request)
    {
        var result = await _updateValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var dispatch = await _db.Dispatches
            .Include(d => d.Vehicles)
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
            dispatch.Price,
            dispatch.PickupDate,
            dispatch.DropoffDate,
            dispatch.DispatchStatus,
            dispatch.Vehicles.Select(v => new DispatchUpdateVehicle(v.Vin))));

        await _db.SaveChangesAsync();

        return DispatchMapper.ToDispatchResponse(dispatch);
    }
}
