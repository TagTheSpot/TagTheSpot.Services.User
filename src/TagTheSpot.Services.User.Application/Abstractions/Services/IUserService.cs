using TagTheSpot.Services.User.Application.DTO;
using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);

        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    }
}
