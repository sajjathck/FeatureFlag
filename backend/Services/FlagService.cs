using FeatureFlags.Api.Data;
using FeatureFlags.Api.DTOs;
using FeatureFlags.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace FeatureFlags.Api.Services
{
    // Application service responsible for managing flags and evaluation logic.
    // This class keeps a simple in-memory cache for read performance and writes through to the DB.
    public class FlagService : IFlagService
    {
        private readonly AppDbContext _db;
        private readonly ConcurrentDictionary<string, FeatureFlag> _cache = new();

        public FlagService(AppDbContext db)
        {
            _db = db;
            LoadCache();
        }

        private void LoadCache()
        {
            foreach (var f in _db.FeatureFlags.AsNoTracking().ToArray())
            {
                _cache[f.Key] = f;
            }
        }

        private void RefreshCache(string key)
        {
            var f = _db.FeatureFlags.AsNoTracking().FirstOrDefault(x => x.Key == key);
            if (f != null) _cache[f.Key] = f;
            else _cache.TryRemove(key, out _);
        }

        public async Task<FlagDto[]> ListAsync()
        {
            // Read from DB for authoritative list; cache is used for evaluation.
            var flags = await _db.FeatureFlags.AsNoTracking().ToArrayAsync();
            return flags.Select(f => new FlagDto(f.Id, f.Name, f.Key, f.Enabled, f.RolloutPercentage, f.TargetUserIds)).ToArray();
        }

        private static string Slugify(string name)
        {
            return string.Concat(name.ToLowerInvariant().Where(c => char.IsLetterOrDigit(c) || c == ' '))
                         .Replace(' ', '_');
        }

        public async Task<FlagDto> CreateAsync(CreateFlagDto dto)
        {
            var key = Slugify(dto.Name);
            var model = new FeatureFlag { Name = dto.Name, Key = key, Enabled = dto.Enabled, RolloutPercentage = Math.Clamp(dto.RolloutPercentage, 0, 100), TargetUserIds = dto.TargetUserIds };
            _db.FeatureFlags.Add(model);
            await _db.SaveChangesAsync();

            // audit
            _db.AuditLogs.Add(new AuditLog { Action = "create", EntityId = model.Id, Details = $"Created flag {model.Key}" });
            await _db.SaveChangesAsync();

            RefreshCache(model.Key);
            return new FlagDto(model.Id, model.Name, model.Key, model.Enabled, model.RolloutPercentage, model.TargetUserIds);
        }

        public async Task<FlagDto?> UpdateAsync(int id, UpdateFlagDto dto)
        {
            var f = await _db.FeatureFlags.FindAsync(id);
            if (f == null) return null;
            if (dto.Name is not null) f.Name = dto.Name;
            if (dto.RolloutPercentage is not null) f.RolloutPercentage = Math.Clamp(dto.RolloutPercentage.Value, 0, 100);
            if (dto.Enabled is not null) f.Enabled = dto.Enabled.Value;
            if (dto.TargetUserIds is not null) f.TargetUserIds = dto.TargetUserIds;
            f.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _db.AuditLogs.Add(new AuditLog { Action = "update", EntityId = f.Id, Details = $"Updated flag {f.Key}" });
            await _db.SaveChangesAsync();

            RefreshCache(f.Key);
            return new FlagDto(f.Id, f.Name, f.Key, f.Enabled, f.RolloutPercentage, f.TargetUserIds);
        }

        public async Task<FlagDto?> ToggleAsync(int id)
        {
            var f = await _db.FeatureFlags.FindAsync(id);
            if (f == null) return null;
            f.Enabled = !f.Enabled;
            f.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _db.AuditLogs.Add(new AuditLog { Action = "toggle", EntityId = f.Id, Details = $"Toggled flag {f.Key} to {f.Enabled}" });
            await _db.SaveChangesAsync();

            RefreshCache(f.Key);
            return new FlagDto(f.Id, f.Name, f.Key, f.Enabled, f.RolloutPercentage, f.TargetUserIds);
        }

        // Evaluation logic (critical):
        // 1) If flag is disabled -> return false with reason 'disabled'.
        // 2) If userId is explicitly targeted (in TargetUserIds list) -> return true with reason 'targeted'.
        // 3) Else evaluate rollout using deterministic hashing: (hash(userId) % 100) < rolloutPercentage -> 'rollout_match' (true) or 'not_in_rollout' (false).
        // Default to false for fail-safe behavior.
        public Task<EvaluateResultDto> EvaluateAsync(string flagKey, string userId)
        {
            try
            {
                if (!_cache.TryGetValue(flagKey, out var f))
                {
                    // Try load from DB if not in cache
                    var dbf = _db.FeatureFlags.AsNoTracking().FirstOrDefault(x => x.Key == flagKey);
                    if (dbf == null) return Task.FromResult(new EvaluateResultDto(flagKey, false, "flag_not_found"));
                    f = dbf;
                    _cache[f.Key] = f;
                }

                if (!f.Enabled)
                {
                    return Task.FromResult(new EvaluateResultDto(f.Name, false, "disabled"));
                }

                // Check targeted users (simple CSV parsing). Exact matching for demo.
                if (!string.IsNullOrWhiteSpace(f.TargetUserIds))
                {
                    var targets = f.TargetUserIds.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    if (targets.Contains(userId))
                    {
                        return Task.FromResult(new EvaluateResultDto(f.Name, true, "targeted"));
                    }
                }

                // Deterministic hash to 0-99
                var hash = 0;
                for (var i = 0; i < userId.Length; i++)
                {
                    hash = (hash * 31 + userId[i]) % 100;
                }

                if (hash < f.RolloutPercentage)
                {
                    return Task.FromResult(new EvaluateResultDto(f.Name, true, "rollout_match"));
                }

                return Task.FromResult(new EvaluateResultDto(f.Name, false, "not_in_rollout"));
            }
            catch
            {
                // Fail-safe: return off
                return Task.FromResult(new EvaluateResultDto(flagKey, false, "error"));
            }
        }
    }
}
