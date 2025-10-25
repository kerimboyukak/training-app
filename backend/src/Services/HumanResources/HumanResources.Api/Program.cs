using Api.Filters;
using Api.MassTransit;
using Api.Swagger;
using HumanResources.AppLogic;
using HumanResources.AppLogic.Events;
using HumanResources.Domain;
using HumanResources.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// register the RabbitMQ service
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmployeeFinishedTrainingEventConsumer>();

    IConfigurationSection rabbitMqSection = builder.Configuration.GetSection("EventBus:RabbitMQ");
    var rabbitMqSettings = new RabbitMqSettings();
    rabbitMqSection.Bind(rabbitMqSettings);
    x.UseRabbitMq(rabbitMqSettings);
});

// Register HumanResourcesContext
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddDbContext<HumanResourcesContext>(options =>
{
    string connectionString = configuration.GetConnectionString("DefaultConnection")!;
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 15,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
#if DEBUG
    options.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddDebug()));
    options.EnableSensitiveDataLogging();
#endif
});

// database migration
builder.Services.AddScoped<HumanResourcesDbInitializer>();


builder.Services.AddScoped<IEmployeeRepository, EmployeeDbRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddSingleton<IEmployeeFactory, Employee.Factory>();

// When an exception occurs in any action method of any controller, filter wil make sure a proper response is returned
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ApplicationExceptionFilterAttribute>();
});
builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ApplicationExceptionFilterAttribute>();
});

// add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        string identityUrl = builder.Configuration.GetValue<string>("Urls:IdentityUrlBackChannel")!;
        options.Authority = identityUrl;
        options.Audience = "hr";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,

            //HACK: the code below bypasses validating the signature of the JWT token. DO NOT DO THIS IN REAL LIFE APPLICATIONS!
            // This is done because in the libraries used to generate and handle tokens must be exactly the same version.
            // The fix proposed in https://docs.duendesoftware.com/identityserver/v7/troubleshooting/wilson/ did not work,
            // so we decided to bypass the signature validation.
            SignatureValidator = (string token, TokenValidationParameters parameters) =>
            {
                var jwt = new JsonWebToken(token);

                return jwt;
            }
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(context.Exception, "Bearer token authentication failed");
                return System.Threading.Tasks.Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                Exception? failure = context.Result?.Failure;
                if (failure is not null)
                {
                    logger.LogWarning(failure, "Bearer token authorization failed");
                }
                else
                {
                    logger.LogWarning("Tried to access forbidden resource");
                }
                return System.Threading.Tasks.Task.CompletedTask;
            }
        };
    });

// all endpoints are only accessible for client applications that have access to the "hr.read" scope
var readPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .RequireClaim("scope", "hr.read")
    .Build();

builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ApplicationExceptionFilterAttribute>();
    options.Filters.Add(new AuthorizeFilter(readPolicy));
});

// endpoints that allow to change the state of the system (write actions) are only accessible for client applications that have access to the "manage" scope
var writePolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .RequireClaim("scope", "manage")
    .Build();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("write", writePolicy);
});


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// OAuth2 support for Swagger
string identityUrl = builder.Configuration.GetValue<string>("Urls:IdentityUrlFrontChannel")!;
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HumanResources.Api", Version = "v1" });
    string securityScheme = "OpenID";
    var scopes = new Dictionary<string, string>
                    {
                        { "hr.read", "HumanResources API - Read access" },
                        { "manage", "Write access" }
                    };
    c.AddSecurityDefinition(securityScheme, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{identityUrl}/connect/authorize"),
                TokenUrl = new Uri($"{identityUrl}/connect/token"),
                Scopes = scopes
            }
        }
    });
    c.OperationFilter<AlwaysAuthorizeOperationFilter>(securityScheme, scopes.Keys.ToArray());
});


var app = builder.Build();

IServiceScope startUpScope = app.Services.CreateScope();
var initializer = startUpScope.ServiceProvider.GetRequiredService<HumanResourcesDbInitializer>();
initializer.MigrateDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HumanResources.Api v1");
        c.OAuthClientId("swagger.hr");
        c.OAuthUsePkce();
    });
}

// Disabled for our mobile development environment --> using an emulator with https will give some problems, so we will use http
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
