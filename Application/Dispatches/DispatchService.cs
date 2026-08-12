using Application;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Dispatches;

public class DispatchService
{
    private readonly IApplicationDbContext _db;
    private readonly IValidator<CreateDispatchRequest> _validator;
    private readonly IValidator<GetDispatchBatchRequest> _batchValidator;
    private readonly IValidator<AssignDriverRequest> _assignDriverValidator;
    private readonly ICurrentUserService _currentUser;

    public DispatchService(
        IApplicationDbContext db,
        IValidator<CreateDispatchRequest> validator,
        IValidator<GetDispatchBatchRequest> batchValidator,
        IValidator<AssignDriverRequest> assignDriverValidator,
        ICurrentUserService currentUser)
    {
        _db = db;
        _validator = validator;
        _batchValidator = batchValidator;
        _assignDriverValidator = assignDriverValidator;
        _currentUser = currentUser;
    }

    public async Task<CreateDispatchResponse> CreateAsync(CreateDispatchRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var dispatch = DispatchMapper.ToDomain(request, _currentUser.UserId);

        _db.Dispatches.Add(dispatch);
        await _db.SaveChangesAsync();

        return DispatchMapper.ToResponse(dispatch);
    }

    // TODO: unit tests deferred - Include()/FirstOrDefaultAsync() need a real
    // query provider (Mock<DbSet<T>> can't fake Include or async LINQ), and
    // covering Drivers mapping needs a way to construct a User in test code
    // (no public constructor/factory exists yet). Revisit with EF InMemory
    // provider + Infrastructure project reference once those are in place.
    public async Task<DispatchResponse> GetByIdAsync(Guid dispatchId)
    {
        // TODO: scope to the current user's company (ShipperId/CarrierId) once
        // ICurrentUserService exposes CompanyId - currently any authorized
        // caller can fetch any dispatch by id.
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

    // TODO: unit tests deferred - same InMemory/User-construction blockers as GetByIdAsync.
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

    // TODO: unit tests deferred - same InMemory/User-construction blockers as GetByIdAsync.
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

    // TODO: unit tests deferred - same InMemory/User-construction blockers as GetByIdAsync.
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

        await _db.SaveChangesAsync();
    }
}
