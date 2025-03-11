using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using UserManagement.Application.Configuration;
using UserManagement.Application.Extensions;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Profiles;
using UserManagement.Application.Services;
using UserManagement.Application.Validator;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpClient();
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

    // Enable JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT Bearer token in the format: Bearer {your-token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    // c.OperationFilter<SecurityRequirementsOperationFilter>();

    // var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

services.AddAutoMapper(typeof(AutoMapperProfile));

services.Configure<KeyCloakConfiguration>(configuration.GetSection(KeyCloakConfiguration.Section));
services.Configure<EmsUi>(configuration.GetSection(EmsUi.Section));
services.Configure<SsoProvidersHintPath>(configuration.GetSection(SsoProvidersHintPath.Section));

services.AddSingleton<ITokenService, TokenService>();
services.AddSingleton<IRestClientService, RestClientService>();

services.AddScoped<IUserService, UserService>();
services.AddScoped<IClientService, ClientService>();
services.AddScoped<IRoleMappingService, RoleMappingService>();
services.AddScoped<IRoleService, RoleService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IPermissionService, PermissionService>();
services.AddScoped<ISsoService, SsoService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<UserRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserRoleRepresentationRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RoleRepresentationRequestValidator>();

var keycloakConfig = configuration.GetSection(KeyCloakConfiguration.Section).Get<KeyCloakConfiguration>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{keycloakConfig.ServerUrl}/realms/{keycloakConfig.Realm}";
        options.Audience = keycloakConfig.ClientId;
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = keycloakConfig.ClientId
        };
    });



var app = builder.Build();
// Enable CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandlerMiddleware();

app.UseHttpsRedirection();


app.MapControllers();

app.Run();
