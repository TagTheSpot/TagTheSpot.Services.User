using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using TagTheSpot.Services.Shared.Messaging.Events.Users;
using TagTheSpot.Services.User.Application.Abstractions.Identity;
using TagTheSpot.Services.User.Application.Abstractions.Services;
using TagTheSpot.Services.User.Application.DTO;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.Application.Options;
using TagTheSpot.Services.User.Domain.Enums;
using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Application.Services
{
    public sealed class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly LoginSettings _loginSettings;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IGoogleAuthService _googleAuthService;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IOptions<LoginSettings> loginSettings,
            IPublishEndpoint publishEndpoint,
            IGoogleAuthService googleAuthService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _loginSettings = loginSettings.Value;
            _publishEndpoint = publishEndpoint;
            _googleAuthService = googleAuthService;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
            }

            var loginResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!loginResult.Succeeded)
            {
                return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokenHash = HashRefreshToken(refreshToken);

            user.RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(
                _loginSettings.RefreshTokenExpiryInDays);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors);

                throw new InvalidOperationException(
                    $"Encountered errors during updating a refresh token for user with email: {user.Email}. Errors: {errors}");
            }

            return Result.Success(
                new LoginResponse(
                    AccessToken: accessToken,
                    RefreshToken: refreshToken));
        }

        public async Task<Result<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshTokenHash = HashRefreshToken(request.RefreshToken);

            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.RefreshTokenHash == refreshTokenHash);

            if (user is null)
            {
                return Result.Failure<LoginResponse>(UserErrors.InvalidRefreshToken);
            }

            if (user.RefreshTokenExpirationTime < DateTime.UtcNow)
            {
                return Result.Failure<LoginResponse>(UserErrors.RefreshTokenExpired);
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokenHash = HashRefreshToken(refreshToken);

            user.RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(
                _loginSettings.RefreshTokenExpiryInDays);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors);

                throw new InvalidOperationException(
                    $"Encountered errors during updating a refresh token for user with email: {user.Email}. Errors: {errors}");
            }

            return Result.Success(
                new LoginResponse(
                    AccessToken: accessToken,
                    RefreshToken: refreshToken));
        }

        private static string HashRefreshToken(string token)
        {
            return Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }

        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var result = await RegisterWithRoleAsync(request, role: Role.RegularUser);

            if (result.IsFailure)
            {
                return Result.Failure<RegisterResponse>(result.Error);
            }

            await _publishEndpoint.Publish(new UserCreatedEvent(
                UserId: Guid.Parse(result.Value.UserId),
                Email: result.Value.Email,
                Role: Role.RegularUser.ToString()));

            return Result.Success(result.Value);
        }

        public async Task<Result<RegisterResponse>> RegisterAdminAsync(RegisterRequest request)
        {
            var result = await RegisterWithRoleAsync(request, role: Role.Admin);

            if (result.IsFailure)
            {
                return Result.Failure<RegisterResponse>(result.Error);
            }

            await _publishEndpoint.Publish(new UserCreatedEvent(
                UserId: Guid.Parse(result.Value.UserId),
                Email: result.Value.Email,
                Role: Role.Admin.ToString()));

            return Result.Success(result.Value);
        }

        private async Task<Result<RegisterResponse>> RegisterWithRoleAsync(
            RegisterRequest request, Role role)
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
                Email = request.Email,
                Role = role
            };

            var registerResult = await _userManager.CreateAsync(user, request.Password);

            if (!registerResult.Succeeded)
            {
                var errorMessage = string.Join("; ", registerResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException($"Failed to register a user with email: {user.Email}. Error message: {errorMessage}");
            }

            return new RegisterResponse(
                UserId: user.Id,
                Email: user.Email);
        }

        public async Task<Result<LoginResponse>> LogInWithGoogleAsync(LogInWithGoogleRequest request)
        {
            var result = await _googleAuthService.ValidateAndGetEmailAsync(request.GoogleIdToken);

            if (result.IsFailure)
            {
                return Result.Failure<LoginResponse>(result.Error);
            }

            var email = result.Value;

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    Email = email,
                    Role = Role.RegularUser,
                    EmailConfirmed = true
                };

                var registerResult = await _userManager.CreateAsync(user);

                if (!registerResult.Succeeded)
                {
                    var errorMessage = string.Join("; ", registerResult.Errors.Select(e => e.Description));

                    throw new InvalidOperationException($"Failed to register a user with email: {user.Email}. Error message: {errorMessage}");
                }

                await _publishEndpoint.Publish(new UserCreatedEvent(
                    UserId: Guid.Parse(user.Id),
                    Email: user.Email,
                    Role: user.Role.ToString()));
            }

            var refreshToken = _tokenService.GenerateRefreshToken();
            var accessToken = _tokenService.GenerateAccessToken(user);

            user.RefreshTokenHash = HashRefreshToken(refreshToken);

            user.RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(
                _loginSettings.RefreshTokenExpiryInDays);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors);

                throw new InvalidOperationException(
                    $"Encountered errors during updating a refresh token for user with email: {user.Email}. Errors: {errors}");
            }

            return new LoginResponse(
                AccessToken: accessToken, 
                RefreshToken: refreshToken);
        }

        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user is null)
            {
                return Result.Failure(UserErrors.NotFound);
            }

            if (user.EmailConfirmed)
            {
                return Result.Failure(UserErrors.EmailAlreadyConfirmed);
            }

            var result = await _userManager.ConfirmEmailAsync(
                user, 
                token: request.Token);

            if (!result.Succeeded)
            {
                return Result.Failure(UserErrors.InvalidEmailConfirmationToken);
            }

            return Result.Success();
        }
    }
}
