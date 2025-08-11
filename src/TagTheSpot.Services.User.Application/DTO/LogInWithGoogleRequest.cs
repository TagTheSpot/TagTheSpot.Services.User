namespace TagTheSpot.Services.User.Application.DTO
{
    public sealed record LogInWithGoogleRequest(
        string GoogleIdToken);
}
