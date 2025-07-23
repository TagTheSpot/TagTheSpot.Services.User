using Microsoft.AspNetCore.Identity;
using TagTheSpot.Services.User.Application.Abstractions.Identity;
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
        private readonly ITokenService _tokenService;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var foundUser = await _userManager.FindByEmailAsync(request.Email);

            if (foundUser is null)
            {
                return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
            }

            var loginResult = await _signInManager.CheckPasswordSignInAsync(foundUser, request.Password, false);

            if (!loginResult.Succeeded)
            {
                return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
            }

            var accessToken = _tokenService.GenerateAccessToken(foundUser);

            return Result.Success(new LoginResponse(accessToken));
        }

        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser is not null)
            {
                return Result.Failure<RegisterResponse>(UserErrors.EmailAlreadyTaken);
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Email,
                Email = request.Email
            };

            var registerResult = await _userManager.CreateAsync(user, request.Password);

            if (!registerResult.Succeeded)
            {
                var errorMessage = string.Join("; ", registerResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException($"Failed to register a user with email: {user.Email}. Error message: {errorMessage}");
            }

            return Result.Success(
                new RegisterResponse(
                    UserId: user.Id, 
                    Email: user.Email));
        }
    }
}
