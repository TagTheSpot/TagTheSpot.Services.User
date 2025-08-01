using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TagTheSpot.Services.User.Application.Abstractions.Services;
using TagTheSpot.Services.User.Application.DTO;
using TagTheSpot.Services.User.WebAPI.Factories;

namespace TagTheSpot.Services.User.WebAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public UsersController(
            IUserService userService,
            ProblemDetailsFactory problemDetailsFactory)
        {
            _userService = userService;
            _problemDetailsFactory = problemDetailsFactory;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            var result = await _userService.RegisterAsync(request);

            return result.IsSuccess ? Ok(result.Value) : _problemDetailsFactory.GetProblemDetails(result);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(
            [FromBody] RegisterRequest request)
        {
            var result = await _userService.RegisterAdminAsync(request);

            return result.IsSuccess ? Ok(result.Value) : _problemDetailsFactory.GetProblemDetails(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            var result = await _userService.LoginAsync(request);

            return result.IsSuccess ? Ok(result.Value) : _problemDetailsFactory.GetProblemDetails(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequest request)
        {
            var result = await _userService.RefreshTokenAsync(request);

            return result.IsSuccess ? Ok(result.Value) : _problemDetailsFactory.GetProblemDetails(result);
        }
    }
}
