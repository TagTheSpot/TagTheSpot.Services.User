using TagTheSpot.Services.User.Application.Abstractions.Identity;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Infrastructure.Services
{
    public sealed class JwtTokenService : ITokenService
    {
        public string GenerateAccessToken(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ValidateAccessTokenAsync(string token, bool validateLifetime = true)
        {
            throw new NotImplementedException();
        }
    }
}
