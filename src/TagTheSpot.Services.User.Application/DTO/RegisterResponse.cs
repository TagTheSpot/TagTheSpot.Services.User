namespace TagTheSpot.Services.User.Application.DTO
{
    public sealed record RegisterResponse(
        Guid UserId,
        string Email);
}
