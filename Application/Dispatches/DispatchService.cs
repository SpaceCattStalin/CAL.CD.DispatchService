using Application;
using FluentValidation;

namespace Application.Dispatches;

public class DispatchService
{
    private readonly IApplicationDbContext _db;
    private readonly IValidator<CreateDispatchRequest> _validator;
    private readonly ICurrentUserService _currentUser;

    public DispatchService(IApplicationDbContext db, IValidator<CreateDispatchRequest> validator, ICurrentUserService currentUser)
    {
        _db = db;
        _validator = validator;
        _currentUser = currentUser;
    }

    public async Task<CreateDispatchResponse> CreateAsync(CreateDispatchRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var dispatch = DispatchMapper.ToDomain(request, _currentUser.ShipperId);

        _db.Dispatches.Add(dispatch);
        await _db.SaveChangesAsync();

        return DispatchMapper.ToResponse(dispatch);
    }
}
