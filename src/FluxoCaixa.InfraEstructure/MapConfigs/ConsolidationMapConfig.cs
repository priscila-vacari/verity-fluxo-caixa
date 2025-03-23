using FluxoCaixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.Infra.MapConfigs
{
    [ExcludeFromCodeCoverage]
    public class ConsolidationMapConfig : IEntityTypeConfiguration<Consolidation>
    {
        public void Configure(EntityTypeBuilder<Consolidation> builder)
        {
            builder.ToTable("consolidations");

            builder.HasKey(k => k.Date);

            builder.Property(p => p.TotalCredit)
                .HasColumnName("total_credit")
                .IsRequired();

            builder.Property(p => p.TotalDebit)
                .HasColumnName("total_dedit")
                .IsRequired();

            builder.Property(p => p.Balance)
                .HasColumnName("balance")
                .IsRequired();
        }
    }
}
