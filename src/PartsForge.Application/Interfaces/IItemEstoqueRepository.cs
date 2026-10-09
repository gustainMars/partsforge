using PartsForge.Domain.Entities;

namespace PartsForge.Application.Interfaces;

public interface IItemEstoqueRepository
{
    Task<ItemEstoque?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ItemEstoque>> ListarAsync(CancellationToken cancellationToken = default);
    Task<bool> ExisteComDescricaoAsync(string descricao, int? ignorandoId = null, CancellationToken cancellationToken = default);
    Task<bool> EstaEmUsoAsync(int id, CancellationToken cancellationToken = default);
    void Adicionar(ItemEstoque item);
    void Remover(ItemEstoque item);
}