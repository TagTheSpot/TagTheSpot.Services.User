using Microsoft.AspNetCore.Identity;
using TagTheSpot.Services.User.Application.Abstractions.Services;
using TagTheSpot.Services.User.Application.DTO;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Application.Services
{
    public sealed class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
