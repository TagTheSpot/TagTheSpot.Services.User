using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TagTheSpot.Services.User.Application.Abstractions.Identity;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.Infrastructure.Authentication.Options;
using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Infrastructure.Services
{
    public sealed class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;

        public JwtTokenService(
            IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _symmetricSecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        }

        public string GenerateAccessToken(ApplicationUser user)
        {
            var claims = GenerateClaims(user);

            var signingCredentials = new SigningCredentials(
                 key: _symmetricSecurityKey,
                 algorithm: _jwtSettings.SecurityAlgorithm);

            var token = GenerateJwt(claims, signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GenerateJwt(
            Claim[] claims,
            SigningCredentials credentials)
        {
            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims,
                notBefore: null,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTimeInMinutes),
                signingCredentials: credentials);
        }

        private static Claim[] GenerateClaims(ApplicationUser user)
        {
            return
            [
                new(JwtRegisteredClaimNames.Sub, user.Id),
                // Email is not expected to be null, as it's used as a login
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new("role", user.Role.ToString())
            ];
        }

        public async Task<Result> ValidateAccessTokenAsync(
            string token, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = GetTokenValidationParameters(
                validateLifetime);

            var validationResult = await tokenHandler.ValidateTokenAsync(
                token, validationParameters);

            if (!validationResult.IsValid)
            {
                return Result.Failure(UserErrors.InvalidAccessToken);
            }

            return Result.Success();
        }

        private TokenValidationParameters GetTokenValidationParameters(
            bool validateLifeTime = true)
        {
            return new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifeTime,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = _symmetricSecurityKey
            };
        }
    }
}
