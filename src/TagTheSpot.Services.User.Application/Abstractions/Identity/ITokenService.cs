using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Application.Abstractions.Identity
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user);

        Task<Result> ValidateAccessTokenAsync(
            string token, bool validateLifetime = true);
    }
}
