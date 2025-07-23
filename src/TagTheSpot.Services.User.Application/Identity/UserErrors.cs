using TagTheSpot.Services.User.SharedKernel.Shared;

namespace TagTheSpot.Services.User.Application.Identity
{
    public static class UserErrors
    {
        public static readonly Error InvalidAccessToken =
           Error.Validation(
               code: "User.InvalidAccessToken",
               description: "The provided access token is invalid.");
    }
}
