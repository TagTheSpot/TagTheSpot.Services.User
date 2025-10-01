using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.User.Application.Identity;

namespace TagTheSpot.Services.User.Application.Abstractions.Identity
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user);

        string GenerateRefreshToken();

        Task<Result> ValidateAccessTokenAsync(
            string token, bool validateLifetime = true);
    }
}
