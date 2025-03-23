using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Infra.MapConfigs;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.Infra.Context
{
    [ExcludeFromCodeCoverage]
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Launch> Launches { get; set; }
        public DbSet<Consolidation> Consolidations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LaunchMapConfig());
            modelBuilder.ApplyConfiguration(new ConsolidationMapConfig());
        }
    }
}
