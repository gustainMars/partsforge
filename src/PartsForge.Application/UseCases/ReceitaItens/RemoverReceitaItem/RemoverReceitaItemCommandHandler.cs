using MediatR;
using PartsForge.Application.Exceptions.Receitas;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.ReceitaItens.RemoverReceitaItem;

public class RemoverReceitaItemCommandHandler(IReceitaRepository repositorio, IUnitOfWork unitOfWork) : IRequestHandler<RemoverReceitaItemCommand>
{
    private readonly IReceitaRepository _repositorio = repositorio;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(RemoverReceitaItemCommand request, CancellationToken cancellationToken)
    {
        var receita = await _repositorio.ObterComItensAsync(request.ReceitaId, cancellationToken)
            ?? throw new ReceitaNaoEncontradaException();
            
        receita.RemoverItem(request.ItemEstoqueId);

        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }
}