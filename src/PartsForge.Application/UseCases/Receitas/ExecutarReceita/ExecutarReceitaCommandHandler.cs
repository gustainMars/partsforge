using MediatR;
using PartsForge.Application.Exceptions;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.Receitas.ExecutarReceita;

public class ExecutarReceitaCommandHandler(IReceitaRepository receitaRepository, IUnitOfWork unitOfWork) : IRequestHandler<ExecutarReceitaCommand, Unit>
{
    private readonly IReceitaRepository _receitaRepository = receitaRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(ExecutarReceitaCommand request, CancellationToken cancellationToken)
    {
        var receita = await _receitaRepository.ObterComItensAsync(request.ReceitaId)
            ?? throw new ReceitaNaoEncontradaException();

        receita.Executar();

        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return Unit.Value;
    }
}