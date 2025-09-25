using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.User.Application.Options
{
    public sealed class EmailLinkGenerationSettings
    {
        public const string SectionName = nameof(EmailLinkGenerationSettings);

        [Required]
        [Url]
        public required string ClientEmailConfirmationLink { get; init; }

        [Required]
        [Url]
        public required string ClientResetPasswordLink { get; init; }
    }
}
