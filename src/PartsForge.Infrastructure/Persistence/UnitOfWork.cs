using PartsForge.Application.Interfaces;

namespace PartsForge.Infrastructure.Persistence;

public class UnitOfWork(PartsForgeDbContext context) : IUnitOfWork
{
    private readonly PartsForgeDbContext _context = context;

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken)
        => _context.SaveChangesAsync(cancellationToken);
}