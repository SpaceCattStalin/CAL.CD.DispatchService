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

        RuleForEach(x => x.Vehicles).ChildRules(vehicle =>
        {
            vehicle.RuleFor(v => v.Make).NotEmpty()
                .WithMessage("Make is required when adding a new vehicle.");
            vehicle.RuleFor(v => v.Model).NotEmpty()
                .WithMessage("Model is required when adding a new vehicle.");
            vehicle.RuleFor(v => v.Year).NotNull()
                .WithMessage("Year is required when adding a new vehicle.");

            vehicle.RuleFor(v => v.Year)
                .GreaterThanOrEqualTo(1900)
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .WithMessage("Year must be between 1900 and next year");

            vehicle.RuleFor(v => v.Vin)
                .Length(10, 15)
                .WithMessage("Vin must be 10 to 15 character length");
        });
    }
}
