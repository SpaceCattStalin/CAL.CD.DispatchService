using System.Text;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Application;
using Application.Auth;
using Application.Dispatches;
using Application.Dispatches.Validator;
using Application.Interfaces;
using DotNetEnv;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Presentation;
using Presentation.Services;

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Extension method for configurate database provider
builder.Services.AddDbConfiguration(builder.Configuration);

// Extension method for configurate authentication and authorization
builder.Services.AddAuthenticationAndAuthorizeConfiguration(builder.Configuration);

// Extension method for configurate the global exception catching middleware
builder.Services.AddCustomExceptionMiddleWareConfiguration();

// Extension method for configurate validators
builder.Services.AddValidatorConfiguration();

// Extension method for configurate cloud infrastructure
builder.Services.AddCloudInfrastructureConfiguration(builder.Configuration);

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
