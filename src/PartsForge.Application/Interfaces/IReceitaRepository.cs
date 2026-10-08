using PartsForge.Domain.Entities;

namespace PartsForge.Application.Interfaces;

public interface IReceitaRepository
{
    Task<Receita?> ObterComItensAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Receita>> ListarComItensAsync();
    void Adicionar(Receita receita);
    void Remover(Receita receita);
}