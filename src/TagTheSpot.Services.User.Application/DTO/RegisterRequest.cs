namespace TagTheSpot.Services.User.Application.DTO
{
    public sealed record RegisterRequest(
        string Email,
        string Password);
}
