namespace TagTheSpot.Services.User.Application.DTO
{
    public sealed record LoginResponse(
        string AccessToken,
        string RefreshToken);
}
