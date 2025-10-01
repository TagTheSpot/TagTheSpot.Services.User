using System.ComponentModel.DataAnnotations;
using TagTheSpot.Services.Shared.Abstractions.Options;

namespace TagTheSpot.Services.User.WebAPI.Options
{
    public sealed class SuperUserSettings : IAppOptions
    {
        public static string SectionName => nameof(SuperUserSettings);

        [EmailAddress]
        [Required]
        public required string Email { get; init; }

        [Required]
        public required string Password { get; init; }
    }
}
