using Microsoft.EntityFrameworkCore;
using PartsForge.Domain.Entities;

namespace PartsForge.Infrastructure.Persistence;

public class PartsForgeDbContext(DbContextOptions<PartsForgeDbContext> options) : DbContext(options)
{
    public DbSet<Receita> Receitas { get; set; }
    public DbSet<ItemEstoque> ItensEstoque { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PartsForgeDbContext).Assembly);
    }
}