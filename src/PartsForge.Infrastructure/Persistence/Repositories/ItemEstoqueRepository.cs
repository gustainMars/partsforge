using Microsoft.EntityFrameworkCore;
using PartsForge.Application.Interfaces;
using PartsForge.Domain.Entities;

namespace PartsForge.Infrastructure.Persistence.Repositories;

public class ItemEstoqueRepository(PartsForgeDbContext context) : IItemEstoqueRepository
{
    private readonly PartsForgeDbContext _context = context;

    public Task<ItemEstoque?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.ItensEstoque.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public Task<List<ItemEstoque>> ListarAsync(CancellationToken cancellationToken = default)
        => _context.ItensEstoque.ToListAsync(cancellationToken);

    public Task<bool> ExisteComDescricaoAsync(string descricao, int? ignorandoId = null, CancellationToken cancellationToken = default)
        => _context.ItensEstoque.AnyAsync(i => i.Descricao == descricao && i.Id != ignorandoId, cancellationToken);

    public Task<bool> EstaEmUsoAsync(int id, CancellationToken cancellationToken = default)
        => _context.Set<ReceitaItem>().AnyAsync(ri => ri.ItemEstoqueId == id, cancellationToken);

    public void Adicionar(ItemEstoque item)
        => _context.ItensEstoque.Add(item);

    public void Remover(ItemEstoque item)
        => _context.ItensEstoque.Remove(item);
}