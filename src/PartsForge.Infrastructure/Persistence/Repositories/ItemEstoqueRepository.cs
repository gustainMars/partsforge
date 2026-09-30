using Microsoft.EntityFrameworkCore;
using PartsForge.Application.Interfaces;
using PartsForge.Domain.Entities;

namespace PartsForge.Infrastructure.Persistence.Repositories;

public class ItemEstoqueRepository(PartsForgeDbContext context) : IItemEstoqueRepository
{
    private readonly PartsForgeDbContext _context = context;

    public Task<ItemEstoque?> ObterPorIdAsync(int id)
        => _context.ItensEstoque.FirstOrDefaultAsync(i => i.Id == id);

    public Task<List<ItemEstoque>> ListarAsync()
        => _context.ItensEstoque.ToListAsync();

    public Task<bool> ExisteComDescricaoAsync(string descricao, int? ignorandoId = null)
        => _context.ItensEstoque.AnyAsync(i => i.Descricao == descricao && i.Id != ignorandoId);

    public Task<bool> EstaEmUsoAsync(int id)
        => _context.Set<ReceitaItem>().AnyAsync(ri => ri.ItemEstoqueId == id);

    public void Adicionar(ItemEstoque item)
        => _context.ItensEstoque.Add(item);

    public void Remover(ItemEstoque item)
        => _context.ItensEstoque.Remove(item);
}