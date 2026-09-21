using Microsoft.EntityFrameworkCore;
using PartsForge.Application.Interfaces;
using PartsForge.Domain.Entities;

namespace PartsForge.Infrastructure.Persistence.Repositories;

public class ReceitaRepository(PartsForgeDbContext context) : IReceitaRepository
{
    private readonly PartsForgeDbContext _context = context;
    public Task<Receita?> ObterComItensAsync(int id)
    {
        return _context.Receitas
            .Include(r => r.Itens)
                .ThenInclude(ri => ri.ItemEstoque)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}