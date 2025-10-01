using TagTheSpot.Services.Shared.Essentials.Results;

namespace TagTheSpot.Services.User.Application.Abstractions.Identity
{
    public interface IGoogleAuthService
    {
        Task<Result<string>> ValidateAndGetEmailAsync(string token);
    }
}
