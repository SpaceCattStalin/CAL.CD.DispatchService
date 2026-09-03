using Application.Dispatches;
using Infrastructure;
using Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
