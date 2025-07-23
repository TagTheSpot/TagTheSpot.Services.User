namespace TagTheSpot.Services.User.Application.Options
{
    public sealed class LoginSettings
    {
        public const string SectionName = nameof(LoginSettings);

        public bool IsPersistent = true;

        public bool LockoutOnFailure = false;

        public int RefreshTokenExpiryInDays = 7;
    }
}
