using FluentValidation;
namespace Application.Dispatches;

public class AssignDriverRequestValidator : AbstractValidator<AssignDriverRequest>
{
    public AssignDriverRequestValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
    }
}
