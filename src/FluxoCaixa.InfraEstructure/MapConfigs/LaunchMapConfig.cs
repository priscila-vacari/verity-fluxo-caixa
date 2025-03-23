using FluxoCaixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.Infra.MapConfigs
{
    [ExcludeFromCodeCoverage]
    public class LaunchMapConfig: IEntityTypeConfiguration<Launch>
    {
        public void Configure(EntityTypeBuilder<Launch> builder)
        {
            builder.ToTable("launches");

            builder.HasKey(k => new { k.Date, k.Type });

            builder.Property(p => p.Amount)
                .HasColumnName("amount")
                .IsRequired();
        }
    }
}
