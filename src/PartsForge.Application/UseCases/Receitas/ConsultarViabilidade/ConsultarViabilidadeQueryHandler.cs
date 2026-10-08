using MediatR;
using PartsForge.Application.Exceptions.Receitas;
using PartsForge.Application.Interfaces;
using PartsForge.Domain.ValueObjects;

namespace PartsForge.Application.UseCases.Receitas.ConsultarViabilidade;

public class ConsultarViabilidadeQueryHandler(IReceitaRepository receitaRepository) : IRequestHandler<ConsultarViabilidadeQuery, IReadOnlyList<ItemViabilidade>>
{
    private readonly IReceitaRepository _receitaRepository = receitaRepository;

    public async Task<IReadOnlyList<ItemViabilidade>> Handle(ConsultarViabilidadeQuery request, CancellationToken cancellationToken)
    {
        var receita = await _receitaRepository.ObterComItensAsync(request.ReceitaId, cancellationToken) ?? throw new ReceitaNaoEncontradaException();
        return receita.VerificarViabilidade();
    }
}