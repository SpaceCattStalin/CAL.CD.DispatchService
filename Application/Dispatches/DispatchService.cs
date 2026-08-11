using Application;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Dispatches;

public class DispatchService
{
    private readonly IApplicationDbContext _db;
    private readonly IValidator<CreateDispatchRequest> _validator;
    private readonly IValidator<GetDispatchBatchRequest> _batchValidator;
    private readonly ICurrentUserService _currentUser;

    public DispatchService(
        IApplicationDbContext db,
        IValidator<CreateDispatchRequest> validator,
        IValidator<GetDispatchBatchRequest> batchValidator,
        ICurrentUserService currentUser)
    {
        _db = db;
        _validator = validator;
        _batchValidator = batchValidator;
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
}
