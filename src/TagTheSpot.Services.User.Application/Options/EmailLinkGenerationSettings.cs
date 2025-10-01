using System.ComponentModel.DataAnnotations;
using TagTheSpot.Services.Shared.Abstractions.Options;

namespace TagTheSpot.Services.User.Application.Options
{
    public sealed class EmailLinkGenerationSettings : IAppOptions
    {
        public static string SectionName => nameof(EmailLinkGenerationSettings);

        [Required]
        [Url]
        public required string ClientEmailConfirmationLink { get; init; }

        [Required]
        [Url]
        public required string ClientResetPasswordLink { get; init; }
    }
}
