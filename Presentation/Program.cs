using System.Text.Json.Serialization;
using Application;
using Application.Auth;
using Application.Dispatches;
using Domain;
using Infrastructure;
using Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string DevelopmentCorsPolicy = "DevelopmentCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(DevelopmentCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOptions<AppSettings>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Extension method for configurate database provider
builder.Services.AddDbConfiguration();

// Extension method for configurate authentication and authorization
builder.Services.AddAuthenticationAndAuthorizeConfiguration();

builder.Services.AddCustomExceptionMiddleWareConfiguration();

builder.Services.AddValidatorConfiguration();

// Extension method for configurate cloud infrastructure
builder.Services.AddCloudInfrastructureConfiguration();

builder.Services.AddScoped<DispatchService>();
builder.Services.AddScoped<CompanyService>();

builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    );
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ShipperOnly", policy => policy.AddRequirements(
        new ShipperOnlyRequirement(CompanyType.Shipper),
        new PermissionAuthorizationRequirement(PermissionNames.DispatchesUpdate))
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors(DevelopmentCorsPolicy);
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
