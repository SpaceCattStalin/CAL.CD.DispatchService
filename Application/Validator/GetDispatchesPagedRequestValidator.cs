using System.Data;
using FluentValidation;
namespace Application.Dispatches.Validator;

public class GetDispatchesPagedRequestValidator : AbstractValidator<GetDispatchesPagedRequest>
{
    public GetDispatchesPagedRequestValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 500)
            .WithMessage("Limit must be between 1 and 500.");

        RuleFor(x => x.Cursor)
            .Must(x => String.IsNullOrEmpty(x) || Guid.TryParse(x, out _))
            .WithMessage("Not valid Guid format");
    }
}
