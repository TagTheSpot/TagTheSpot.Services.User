using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.User.WebAPI.Options
{
    public sealed class SuperUserSettings
    {
        public const string SectionName = nameof(SuperUserSettings);

        [EmailAddress]
        [Required]
        public required string Email { get; init; }

        [Required]
        public required string Password { get; init; }
    }
}
