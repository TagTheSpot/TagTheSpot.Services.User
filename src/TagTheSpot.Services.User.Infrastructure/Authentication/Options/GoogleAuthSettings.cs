using System.ComponentModel.DataAnnotations;
using TagTheSpot.Services.Shared.Abstractions.Options;

namespace TagTheSpot.Services.User.Infrastructure.Authentication.Options
{
    public sealed class GoogleAuthSettings : IAppOptions
    {
        public static string SectionName => nameof(GoogleAuthSettings);

        [Required]
        public required string ClientId { get; init; }
    }
}
