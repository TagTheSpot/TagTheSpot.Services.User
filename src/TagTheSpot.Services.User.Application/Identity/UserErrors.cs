using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Application.Identity
{
    public static class UserErrors
    {
        public static readonly Error NotFound =
            Error.NotFound(
                code: "User.NotFound",
                description: "The user has not been found.");

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

        public static readonly Error EmailAlreadyConfirmed =
            Error.Validation(
                code: "User.EmailAlreadyConfirmed",
                description: "The email is already confirmed for this user.");

        public static readonly Error InvalidEmailConfirmationToken =
            Error.Validation(
                code: "User.InvalidEmailConfirmationToken",
                description: "The provided email confirmation token is invalid or expired.");
    }
}
