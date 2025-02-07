using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using UserManagement.Application.Configuration;
using UserManagement.Application.Dtos;
using UserManagement.Application.Extensions;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Profiles;
using UserManagement.Application.Services;
using UserManagement.Application.Validator;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1",
        Description = "User Management API",
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

services.AddAutoMapper(typeof(AutoMapperProfile));

services.Configure<KeyCloakConfiguration>(configuration.GetSection(KeyCloakConfiguration.Section));
services.AddSingleton<ITokenService, TokenService>();
services.AddSingleton<IRestClientService, RestClientService>();

services.AddScoped<IUserService, UserService>();
services.AddScoped<IClientService, ClientService>();
services.AddScoped<IRoleMappingService, RoleMappingService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<UserRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserRoleRepresentationRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RoleRepresentationRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandlerMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
