using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TagTheSpot.Services.User.Application.Abstractions.Identity;
using TagTheSpot.Services.User.Application.Abstractions.Services;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.Application.Options;
using TagTheSpot.Services.User.Application.Services;
using TagTheSpot.Services.User.Application.Validators;
using TagTheSpot.Services.User.Infrastructure.Authentication.Options;
using TagTheSpot.Services.User.Infrastructure.Extensions;
using TagTheSpot.Services.User.Infrastructure.Options;
using TagTheSpot.Services.User.Infrastructure.Persistence;
using TagTheSpot.Services.User.Infrastructure.Persistence.Options;
using TagTheSpot.Services.User.Infrastructure.Services;
using TagTheSpot.Services.User.WebAPI.Extensions;
using TagTheSpot.Services.User.WebAPI.Factories;
using TagTheSpot.Services.User.WebAPI.Middleware;
using TagTheSpot.Services.User.WebAPI.Options;

namespace TagTheSpot.Services.User.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            builder.Services.AddOptions<RabbitMqSettings>()
                .BindConfiguration(RabbitMqSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<JwtSettings>()
                .BindConfiguration(JwtSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<DbSettings>()
                .BindConfiguration(DbSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<LoginSettings>()
                .BindConfiguration(LoginSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<SuperUserSettings>()
                .BindConfiguration(SuperUserSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<GoogleAuthSettings>()
                .BindConfiguration(GoogleAuthSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

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

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigureSwaggerGen();

            builder.Services.ConfigureAuthentication();

            builder.Services.AddCorsPolicies();

            var app = builder.Build();

            app.UseExceptionHandlingMiddleware();

            if (app.Environment.IsDevelopment())
            {
                app.UseCors(CorsExtensions.DevelopmentPolicyName);
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.CreateSuperUserIfNotCreated().Wait();

            app.Run();
        }
    }
}
