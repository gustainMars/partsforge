using MediatR;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.ItensEstoque.AdicionarEstoque;

public class AdicionarEstoqueItemEstoqueCommandHandler(IItemEstoqueRepository repository, IUnitOfWork unitOfWork) 
    : IRequestHandler<AdicionarEstoqueItemEstoqueCommand>
{
    private readonly IItemEstoqueRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(AdicionarEstoqueItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new ItemEstoqueNaoEncontradoException();
        
        item.Incrementar(request.Quantidade);

        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }
}