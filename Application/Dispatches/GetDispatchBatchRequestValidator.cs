using FluentValidation;
namespace Application.Dispatches;

public class GetDispatchBatchRequestValidator : AbstractValidator<GetDispatchBatchRequest>
{
    public GetDispatchBatchRequestValidator()
    {
        RuleFor(x => x.DispatchIds)
            .NotEmpty()
            .WithMessage("At least one DispatchId is required.");
    }
}
