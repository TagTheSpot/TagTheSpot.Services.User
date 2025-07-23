namespace TagTheSpot.Services.User.Application.DTO
{
    public sealed record RegisterResponse(
        string UserId,
        string Email);
}
