namespace TagTheSpot.Services.User.Application.DTO
{
    public sealed record LoginRequest(
        string Email,
        string Password);
}
