using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Application.Identity
{
    public static class UserErrors
    {
        public static readonly Error InvalidAccessToken =
           Error.Validation(
               code: "User.InvalidAccessToken",
               description: "The provided access token is invalid.");

        public static readonly Error EmailAlreadyTaken =
            Error.Validation(
                code: "User.EmailAlreadyTaken",
                description: "The same email is already taken. Please, use a different one.");

        public static readonly Error InvalidCredentials =
            Error.Validation(
                code: "User.InvalidCredentials",
                description: "The provided credentials are invalid.");

        public static readonly Error InvalidRefreshToken =
            Error.Validation(
                code: "User.InvalidRefreshToken",
                description: "The provided refresh token is invalid.");

        public static readonly Error RefreshTokenExpired =
            Error.Validation(
                code: "User.RefreshTokenExpired",
                description: "The provided refresh token has expired.");

        public static readonly Error InvalidGoogleIdToken =
            Error.Validation(
                code: "User.InvalidGoogleIdToken",
                description: "The provided ID token is invalid.");
    }
}
