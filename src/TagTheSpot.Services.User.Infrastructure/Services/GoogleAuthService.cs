using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using TagTheSpot.Services.User.Application.Abstractions.Identity;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.Infrastructure.Authentication.Options;
using TagTheSpot.Services.User.SharedKernel.Shared;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace TagTheSpot.Services.User.Infrastructure.Services
{
    public sealed class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleAuthSettings _settings;

        public GoogleAuthService(
            IOptions<GoogleAuthSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<Result<string>> ValidateAndGetEmailAsync(string token)
        {
            var validationSettings = new ValidationSettings()
            {
                Audience = [_settings.ClientId],
            };

            try
            {
                var payload = await ValidateAsync(
                    jwt: token,
                    validationSettings);

                return payload.Email;
            }
            catch (InvalidJwtException)
            {
                return Result.Failure<string>(UserErrors.InvalidGoogleIdToken);
            }
        }
    }
}
