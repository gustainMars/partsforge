using MediatR;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.UseCases.ItensEstoque.CriarItemEstoque;

public class CriarItemEstoqueCommandHandler(IItemEstoqueRepository repository, IUnitOfWork unitOfWork)
: IRequestHandler<CriarItemEstoqueCommand, int>
{
    private readonly IItemEstoqueRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<int> Handle(CriarItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = new ItemEstoque(request.Descricao, request.Quantidade);

        if (await _repository.ExisteComDescricaoAsync(item.Descricao))
            throw new DescricaoJaCadastradaException();

        _repository.Adicionar(item);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return item.Id;
    }
}