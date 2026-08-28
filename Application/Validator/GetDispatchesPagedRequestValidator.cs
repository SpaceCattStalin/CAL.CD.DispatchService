using FluentValidation;
namespace Application.Dispatches.Validator;

public class GetDispatchesPagedRequestValidator : AbstractValidator<GetDispatchesPagedRequest>
{
    public GetDispatchesPagedRequestValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 500)
            .WithMessage("Limit must be between 1 and 500.");
    }
}
