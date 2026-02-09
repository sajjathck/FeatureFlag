namespace FeatureFlags.Api.DTOs
{
    public record FlagDto(int Id, string Name, string Key, bool Enabled, int RolloutPercentage, string? TargetUserIds);
    public record CreateFlagDto(string Name, int RolloutPercentage = 0, bool Enabled = false, string? TargetUserIds = null);
    public record UpdateFlagDto(string? Name, int? RolloutPercentage, bool? Enabled, string? TargetUserIds);
    public record EvaluateResultDto(string Feature, bool Enabled, string Reason);
}
