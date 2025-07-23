using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using TagTheSpot.Services.User.Application.Abstractions.Identity;
using TagTheSpot.Services.User.Application.Abstractions.Services;
using TagTheSpot.Services.User.Application.DTO;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.Application.Options;
using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Application.Services
{
    public sealed class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly LoginSettings _loginSettings;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IOptions<LoginSettings> loginSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _loginSettings = loginSettings.Value;
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
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenHash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));

            foundUser.RefreshTokenHash = refreshTokenHash;

            foundUser.RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(
                _loginSettings.RefreshTokenExpiryInDays);

            await _userManager.UpdateAsync(foundUser);

            return Result.Success(
                new LoginResponse(
                    AccessToken: accessToken,
                    RefreshToken: refreshToken));
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
