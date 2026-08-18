using FluentValidation;
namespace Application.Dispatches.Validator;

public class GetDispatchBatchRequestValidator : AbstractValidator<GetDispatchBatchRequest>
{
    public GetDispatchBatchRequestValidator()
    {
        RuleFor(x => x.DispatchIds)
            .NotEmpty()
            .WithMessage("At least one DispatchId is required.");
    }
}
