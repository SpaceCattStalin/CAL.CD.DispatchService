using FluentValidation;
namespace Application.Dispatches;

public class UpdateDispatchRequestValidator : AbstractValidator<UpdateDispatchRequest>
{
    public UpdateDispatchRequestValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.PickupDate).GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.DropoffDate).GreaterThan(x => x.PickupDate);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.PickupStop).SetValidator(new StopRequestValidator());
        RuleFor(x => x.DropoffStop).SetValidator(new StopRequestValidator());

        RuleFor(x => x.Vehicles)
            .Must(v => v.Count() >= 1 && v.Count() <= 12)
            .WithMessage("A dispatch must have between 1 and 12 vehicles.");

        RuleFor(x => x.Vehicles)
            .Must(v => v.Where(x => x.VehicleId.HasValue).Select(x => x.VehicleId)
                        .Distinct().Count() == v.Count(x => x.VehicleId.HasValue))
            .WithMessage("Duplicate VehicleId in Vehicles.");

        RuleForEach(x => x.Vehicles).ChildRules(vehicle =>
        {
            vehicle.RuleFor(v => v.Make).NotEmpty().When(v => v.VehicleId is null)
                .WithMessage("Make is required when adding a new vehicle.");
            vehicle.RuleFor(v => v.Model).NotEmpty().When(v => v.VehicleId is null)
                .WithMessage("Model is required when adding a new vehicle.");
            vehicle.RuleFor(v => v.Year).NotNull().When(v => v.VehicleId is null)
                .WithMessage("Year is required when adding a new vehicle.");
        });
    }
}
