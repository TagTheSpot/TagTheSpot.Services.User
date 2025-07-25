using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.User.Infrastructure.Options
{
    public sealed class AzureServiceBusSettings
    {
        public const string SectionName = nameof(AzureServiceBusSettings);

        [Required]
        public required string ConnectionString { get; init; }

        [Required]
        public required string UserTopicName { get; init; }
    }
}
