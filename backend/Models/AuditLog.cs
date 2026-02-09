using System.ComponentModel.DataAnnotations;

namespace FeatureFlags.Api.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        public string Action { get; set; } = null!; // toggle/update/create
        public string Entity { get; set; } = "FeatureFlag";
        public int EntityId { get; set; }
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
