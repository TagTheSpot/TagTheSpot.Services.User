namespace TagTheSpot.Services.User.Application.DTO
{
    public sealed record ConfirmEmailRequest(
        Guid UserId,
        string Token);
}