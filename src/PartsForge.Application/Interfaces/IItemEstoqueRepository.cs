using PartsForge.Domain.Entities;

namespace PartsForge.Application.Interfaces;

public interface IItemEstoqueRepository
{
    Task<ItemEstoque?> ObterPorIdAsync(int id);
    Task<List<ItemEstoque>> ListarAsync();
    Task<bool> ExisteComDescricaoAsync(string descricao, int? ignorandoId = null);
    Task<bool> EstaEmUsoAsync(int id);
    void Adicionar(ItemEstoque item);
    void Remover(ItemEstoque item);
}