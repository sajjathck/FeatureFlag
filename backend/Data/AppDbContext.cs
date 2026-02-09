using FeatureFlags.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FeatureFlags.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FeatureFlag> FeatureFlags { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeatureFlag>().HasKey(f => f.Id);
            modelBuilder.Entity<AuditLog>().HasKey(a => a.Id);
        }
    }
}
