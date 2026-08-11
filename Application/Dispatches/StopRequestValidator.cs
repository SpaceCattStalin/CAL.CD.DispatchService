using FluentValidation;

namespace Application.Dispatches;

public class StopRequestValidator : AbstractValidator<StopRequest>
{
    public StopRequestValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty()
            .Length(20, 100);
        RuleFor(x => x.LocationName)
            .Length(10, 30);
        RuleFor(x => x.ContactName)
            .Length(5, 50);
        RuleFor(x => x.ContactPhone)
            .Length(10, 12);
        RuleFor(x => x.ContactEmail)
            .Length(15, 30)
        // Only check for the presence of "@". Reference: following the EmailAddress() function documentation
        .EmailAddress();
    }
}
