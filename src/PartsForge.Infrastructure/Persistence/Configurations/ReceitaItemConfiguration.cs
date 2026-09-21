using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartsForge.Domain.Entities;

namespace PartsForge.Infrastructure.Persistence.Configurations;

public class ReceitaItemConfiguration : IEntityTypeConfiguration<ReceitaItem>
{
    public void Configure(EntityTypeBuilder<ReceitaItem> builder)
    {
        builder.ToTable("ReceitaItens");

        builder.HasKey(ri => new { ri.ReceitaId, ri.ItemEstoqueId});

        builder.HasOne(ri => ri.ItemEstoque)
            .WithMany()
            .HasForeignKey(ri => ri.ItemEstoqueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Receita>()
            .WithMany(r => r.Itens)
            .HasForeignKey(ri => ri.ReceitaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}