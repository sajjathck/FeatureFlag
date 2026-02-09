using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeatureFlags.Api.Models
{
    // Domain entity representing a feature flag.
    public class FeatureFlag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        // Generated key (slug) used for evaluation and APIs.
        [Required]
        public string Key { get; set; } = null!;

        public bool Enabled { get; set; }

        // 0-100
        public int RolloutPercentage { get; set; }

        // Comma-separated list of targeted user ids (simple approach for demo)
        public string? TargetUserIds { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
