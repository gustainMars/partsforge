using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PartsForge.Infrastructure.Persistence;

public class PartsForgeDbContextFactory : IDesignTimeDbContextFactory<PartsForgeDbContext>
{
    public PartsForgeDbContext CreateDbContext(string[] args)
    {
        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")
            ?? throw new InvalidOperationException("Defina a variável de ambiente MSSQL_SA_PASSWORD antes de rodar comandos do EF Core.");
        
        var optionsBuilder = new DbContextOptionsBuilder<PartsForgeDbContext>();
        optionsBuilder.UseSqlServer($"Server=localhost;Database=PartsForge;User Id=sa;Password={password};TrustServerCertificate=True;");

        return new PartsForgeDbContext(optionsBuilder.Options);
    }
}