using Microsoft.EntityFrameworkCore;
using PartsForge.Application.Interfaces;
using PartsForge.Domain.Entities;

namespace PartsForge.Infrastructure.Persistence.Repositories;

public class ReceitaRepository(PartsForgeDbContext context) : IReceitaRepository
{
    private readonly PartsForgeDbContext _context = context;

    public void Adicionar(Receita receita)
    {
        _context.Receitas.Add(receita);
    }

    public Task<List<Receita>> ListarComItensAsync()
    {
        return _context.Receitas
            .Include(r => r.Itens)
                .ThenInclude(ri => ri.ItemEstoque)
            .ToListAsync();
    }

    public Task<Receita?> ObterComItensAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Receitas
            .Include(r => r.Itens)
                .ThenInclude(ri => ri.ItemEstoque)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public void Remover(Receita receita)
    {
        _context.Receitas.Remove(receita);
    }
}