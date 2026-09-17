namespace PartsForge.Application.Interfaces;

public interface IUnitOfWork
{
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken);
}