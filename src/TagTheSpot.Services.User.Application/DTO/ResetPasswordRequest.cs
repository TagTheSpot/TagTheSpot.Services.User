namespace TagTheSpot.Services.User.Application.DTO
{
    public sealed record ResetPasswordRequest(
        string Email,
        string Token,
        string NewPassword);
}
