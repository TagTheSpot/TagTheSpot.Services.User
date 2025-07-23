using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TagTheSpot.Services.User.Application.Abstractions.Services;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.Application.Services;
using TagTheSpot.Services.User.Infrastructure.Authentication.Options;
using TagTheSpot.Services.User.Infrastructure.Persistence;
using TagTheSpot.Services.User.Infrastructure.Persistence.Options;
using TagTheSpot.Services.User.WebAPI.Factories;
using TagTheSpot.Services.User.WebAPI.Middleware;

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
            })
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddOptions<JwtSettings>()
                .BindConfiguration(JwtSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddSingleton<ProblemDetailsFactory>();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseExceptionHandlingMiddleware();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
