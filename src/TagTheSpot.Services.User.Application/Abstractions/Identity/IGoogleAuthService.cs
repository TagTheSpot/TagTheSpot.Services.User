using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Application.Abstractions.Identity
{
    public interface IGoogleAuthService
    {
        Task<Result<string>> ValidateAndGetEmailAsync(string token);
    }
}
