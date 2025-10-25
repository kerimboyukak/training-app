using Api.Filters;
using Api.MassTransit;
using Api.Swagger;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using Training.AppLogic;
using Training.AppLogic.Events;
using Training.Infrastructure;


namespace Training.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IApprenticeRepository, ApprenticeDbRepository>();
            builder.Services.AddScoped<ICoachRepository, CoachDbRepository>();
            builder.Services.AddScoped<ITrainingRepository, TrainingDbRepository>();
            builder.Services.AddScoped<IRoomRepository, RoomDbRepository>();
            builder.Services.AddScoped<IApprenticeService, ApprenticeService>();
            builder.Services.AddScoped<ICoachService, CoachService>();

            builder.Services.AddScoped<TrainingDbInitializer>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            // Register MassTransit
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<EmployeeAppointedAsCoachEventConsumer>();
                x.AddConsumer<EmployeeHiredForTrainingEventConsumer>();

                IConfigurationSection rabbitMqSection = builder.Configuration.GetSection("EventBus:RabbitMQ");
                var rabbitMqSettings = new RabbitMqSettings();
                rabbitMqSection.Bind(rabbitMqSettings); // the settings in appssettings.json are bound to the rabbitMqSettings
                x.UseRabbitMq(rabbitMqSettings);        // using the RabbitMQ settings from the Api project in SharedKernel
            });

            //Register TrainingContext
            ConfigurationManager configuration = builder.Configuration;
            builder.Services.AddDbContext<TrainingContext>(options =>
            {
                string connectionString = configuration.GetConnectionString("DefaultConnection")!;
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            #if DEBUG
                options.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddDebug()));
                options.EnableSensitiveDataLogging();
            #endif
            });

            // Adding applicationExceptionFilterAttribute
            builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));
            builder.Services.AddControllers(options =>
            {
                options.Filters.AddService<ApplicationExceptionFilterAttribute>();
            });

            // Adding authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    string identityUrl = builder.Configuration.GetValue<string>("Urls:IdentityUrlBackChannel")!;
                    options.Authority = identityUrl;
                    options.Audience = "training";
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

            // adds a policy that enforces ALL controller actions to have an authenticated user that has a "scope" claim with the value "training.read"
            var readPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim("scope", "training.read")
                .Build();
            builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));
            builder.Services.AddControllers(options =>
            {
                options.Filters.AddService<ApplicationExceptionFilterAttribute>();
                options.Filters.Add(new AuthorizeFilter(readPolicy));
            });

            // secure controller actions that need write access, requires a user to be authenticated and have a "scope" claim with the value "manage"
            var writePolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim("scope", "manage")
                .Build();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("write", writePolicy);
            });



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // OAuth2 support for Swagger
            string identityUrl = builder.Configuration.GetValue<string>("Urls:IdentityUrlFrontChannel")!;
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Training.Api", Version = "v1" });
                string securityScheme = "OpenID";
                var scopes = new Dictionary<string, string>
                    {
                        { "training.read", "Training API - Read access" },
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

            // Using the TrainingDbInitializer to migrate and seed the database at startup
            IServiceScope startUpScope = app.Services.CreateScope();
            var initializer = startUpScope.ServiceProvider.GetRequiredService<TrainingDbInitializer>();
            initializer.MigrateDatabase();
            initializer.SeedData();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                // Add login ui components
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Training.Api v1");
                    c.OAuthClientId("swagger.training");
                    c.OAuthUsePkce();
                });
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
