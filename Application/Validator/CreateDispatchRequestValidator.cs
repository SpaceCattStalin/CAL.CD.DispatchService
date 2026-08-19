using FluentValidation;
namespace Application.Dispatches.Validator;

public class CreateDispatchRequestValidator : AbstractValidator<CreateDispatchRequest>
{
    public CreateDispatchRequestValidator()
    {
        RuleFor(x => x.CarrierId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.PickupDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.DropoffDate)
            .GreaterThan(DateTime.UtcNow)
            .GreaterThan(x => x.PickupDate);
        RuleFor(x => x.PickupStop).SetValidator(new StopRequestValidator());
        RuleFor(x => x.DropoffStop).SetValidator(new StopRequestValidator());
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Vehicles)
            .Must(x => x.Count() >= 1 && x.Count() <= 12)
            .WithMessage("A dispatch must have between 1 and 12 vehicles.");
    }
}
