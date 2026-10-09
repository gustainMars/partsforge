using MediatR;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.ItensEstoque.RemoverItemEstoque;

public class RemoverItemEstoqueCommandHandler(IItemEstoqueRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<RemoverItemEstoqueCommand>
{
    private readonly IItemEstoqueRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(RemoverItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new ItemEstoqueNaoEncontradoException();

        if (await _repository.EstaEmUsoAsync(item.Id, cancellationToken))
            throw new ItemEstoqueEmUsoException();

        _repository.Remover(item);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }
}