using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TagTheSpot.Services.User.Infrastructure.Authentication.Options;

namespace TagTheSpot.Services.User.WebAPI.Extensions
{
    internal static class AuthenticationExtensions
    {
        public static IServiceCollection ConfigureAuthentication(
            this IServiceCollection services)
        {
            var jwtSettings = services.BuildServiceProvider().GetRequiredService<IOptions<JwtSettings>>().Value;
            var signingKey = CreateSigningKey(jwtSettings);

            services.AddAuthenticationWithJwtBearer(jwtSettings, signingKey);

            return services;
        }

        private static SymmetricSecurityKey CreateSigningKey(JwtSettings jwtSettings)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        }

        private static IServiceCollection AddAuthenticationWithJwtBearer(
            this IServiceCollection services,
            JwtSettings jwtSettings,
            SymmetricSecurityKey signingKey)
        {
            services.AddAuthentication(
                ConfigureAuthenticationOptions())
            .AddJwtBearer(
                ConfigureJwtBearerOptions(jwtSettings, signingKey));

            return services;
        }

        private static Action<AuthenticationOptions> ConfigureAuthenticationOptions()
        {
            return options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            };
        }

        private static Action<JwtBearerOptions> ConfigureJwtBearerOptions(
            JwtSettings jwtSettings,
            SymmetricSecurityKey signingKey)
        {
            return options =>
            {
                options.TokenValidationParameters = CreateTokenValidationParameters(
                    jwtSettings,
                    signingKey);
            };
        }

        private static TokenValidationParameters CreateTokenValidationParameters(
            JwtSettings jwtSettings,
            SymmetricSecurityKey signingKey)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = signingKey,
                RoleClaimType = ClaimTypes.Role
            };
        }
    }
}
