using TagTheSpot.Services.Shared.Abstractions.Options;

namespace TagTheSpot.Services.User.Application.Options
{
    public sealed class LoginSettings : IAppOptions
    {
        public static string SectionName => nameof(LoginSettings);

        public bool IsPersistent = true;

        public bool LockoutOnFailure = false;

        public int RefreshTokenExpiryInDays = 7;
    }
}
