using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.User.Application.Options
{
    public sealed class EmailLinkGenerationSettings
    {
        public const string SectionName = nameof(EmailLinkGenerationSettings);

        [Required]
        [Url]
        public required string ClientConfirmationLink { get; init; }
    }
}
