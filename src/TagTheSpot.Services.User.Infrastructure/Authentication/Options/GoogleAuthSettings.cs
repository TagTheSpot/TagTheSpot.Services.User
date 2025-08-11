using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.User.Infrastructure.Authentication.Options
{
    public sealed class GoogleAuthSettings
    {
        public const string SectionName = nameof(GoogleAuthSettings);

        [Required]
        public required string ClientId { get; init; }
    }
}
