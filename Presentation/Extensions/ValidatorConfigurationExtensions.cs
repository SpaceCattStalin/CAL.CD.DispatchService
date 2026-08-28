using Application.Auth;
using Application.Dispatches;
using Application.Dispatches.Validator;
using FluentValidation;

namespace Presentation;

public static class ValidatorConfigurationExtensions
{
    public static IServiceCollection AddValidatorConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateDispatchRequest>, CreateDispatchRequestValidator>();
        services.AddScoped<IValidator<GetDispatchBatchRequest>, GetDispatchBatchRequestValidator>();
        services.AddScoped<IValidator<AssignDriverRequest>, AssignDriverRequestValidator>();
        services.AddScoped<IValidator<UpdateDispatchRequest>, UpdateDispatchRequestValidator>();
        services.AddScoped<IValidator<GetDispatchesPagedRequest>, GetDispatchesPagedRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

        return services;
    }
}
