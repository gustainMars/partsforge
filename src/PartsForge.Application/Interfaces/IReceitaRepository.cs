using PartsForge.Domain.Entities;

namespace PartsForge.Application.Interfaces;

public interface IReceitaRepository
{
    Task<Receita?> ObterComItensAsync(int id);
}