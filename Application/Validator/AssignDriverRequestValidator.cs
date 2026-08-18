using FluentValidation;
namespace Application.Dispatches.Validator;

public class AssignDriverRequestValidator : AbstractValidator<AssignDriverRequest>
{
    public AssignDriverRequestValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
    }
}
