using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartsForge.Domain.Entities;

namespace PartsForge.Infrastructure.Persistence.Configurations;

public class ItemEstoqueConfiguration : IEntityTypeConfiguration<ItemEstoque>
{
    public void Configure(EntityTypeBuilder<ItemEstoque> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Descricao)
            .IsRequired()
            .HasMaxLength(200);
    }
}