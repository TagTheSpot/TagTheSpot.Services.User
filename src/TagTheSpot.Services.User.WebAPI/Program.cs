using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using TagTheSpot.Services.Shared.API.DependencyInjection;
using TagTheSpot.Services.Shared.API.Factories;
using TagTheSpot.Services.Shared.API.Middleware;
using TagTheSpot.Services.Shared.Application.Extensions;
using TagTheSpot.Services.Shared.Auth.DependencyInjection;
using TagTheSpot.Services.Shared.Auth.Options;
using TagTheSpot.Services.Shared.Infrastructure.Options;
using TagTheSpot.Services.User.Application.Abstractions.Identity;
using TagTheSpot.Services.User.Application.Abstractions.Services;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.Application.Options;
using TagTheSpot.Services.User.Application.Services;
using TagTheSpot.Services.User.Application.Validators;
using TagTheSpot.Services.User.Infrastructure.Authentication.Options;
using TagTheSpot.Services.User.Infrastructure.Extensions;
using TagTheSpot.Services.User.Infrastructure.Persistence;
using TagTheSpot.Services.User.Infrastructure.Services;
using TagTheSpot.Services.User.WebAPI.Extensions;
using TagTheSpot.Services.User.WebAPI.Options;

namespace TagTheSpot.Services.User.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var swaggerXmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var swaggerXmlFilePath = Path.Combine(AppContext.BaseDirectory, swaggerXmlFileName);

            builder.Services.ConfigureSwaggerGen(swaggerXmlFilePath);

            builder.Services.AddDbContext<ApplicationDbContext>(
                (serviceProvider, options) =>
                {
                    var dbSettings = serviceProvider.GetRequiredService<IOptions<DbSettings>>().Value;

                    options.UseNpgsql(dbSettings.ConnectionString);
                });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Tokens.AuthenticatorTokenProvider = "Default";
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureValidatableOnStartOptions<RabbitMqSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<JwtSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<DbSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<LoginSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<SuperUserSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<GoogleAuthSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<EmailLinkGenerationSettings>();

            builder.Services.AddSingleton<ProblemDetailsFactory>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, JwtTokenService>();
            builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    RabbitMqSettings settings = context
                        .GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                    configurator.Host(settings.Host, settings.VirtualHost, h =>
                    {
                        h.Username(settings.Username);
                        h.Password(settings.Password);
                    });
                });
            });

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            builder.Services.ConfigureAuthentication();

            builder.Services.AddDevelopmentCorsPolicy();

            var app = builder.Build();

            app.UseExceptionHandlingMiddleware();

            if (app.Environment.IsDevelopment())
            {
                app.UseCors(CorsExtensions.DevelopmentPolicyName);
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.ApplyMigrations();
            app.CreateSuperUserIfNotCreated().Wait();

            app.Run();
        }
    }
}
