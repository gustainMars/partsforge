using MediatR;
using PartsForge.Application.Exceptions.Receitas;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.ReceitaItens.AlterarQuantidade;

public class AlterarQuantidadeReceitaItemCommandHandler(IReceitaRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<AlterarQuantidadeReceitaItemCommand>
{
    private readonly IReceitaRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(AlterarQuantidadeReceitaItemCommand request, CancellationToken cancellationToken)
    {
        var receita = await _repository.ObterComItensAsync(request.ReceitaId, cancellationToken)
            ?? throw new ReceitaNaoEncontradaException();

        receita.AlterarQuantidadeItem(request.ItemEstoqueId, request.NovaQuantidade);

        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }
}