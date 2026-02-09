using FeatureFlags.Api.DTOs;
using FeatureFlags.Api.Models;

namespace FeatureFlags.Api.Services
{
    public interface IFlagService
    {
        Task<FlagDto[]> ListAsync();
        Task<FlagDto> CreateAsync(CreateFlagDto dto);
        Task<FlagDto?> UpdateAsync(int id, UpdateFlagDto dto);
        Task<FlagDto?> ToggleAsync(int id);
        Task<EvaluateResultDto> EvaluateAsync(string flagKey, string userId);
    }
}
