using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartsForge.Domain.Entities;

namespace PartsForge.Infrastructure.Persistence.Configurations;

public class ReceitaConfiguration : IEntityTypeConfiguration<Receita>
{
    public void Configure(EntityTypeBuilder<Receita> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Descricao)
            .IsRequired()
            .HasMaxLength(200);
    }
}