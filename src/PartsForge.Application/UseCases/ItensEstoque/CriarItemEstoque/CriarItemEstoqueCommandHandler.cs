using MediatR;
using PartsForge.Application.Dtos;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.UseCases.ItensEstoque.CriarItemEstoque;

public class CriarItemEstoqueCommandHandler(IItemEstoqueRepository repository, IUnitOfWork unitOfWork)
: IRequestHandler<CriarItemEstoqueCommand, ItemEstoqueDto>
{
    private readonly IItemEstoqueRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<ItemEstoqueDto> Handle(CriarItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = new ItemEstoque(request.Descricao, request.Quantidade);

        if (await _repository.ExisteComDescricaoAsync(item.Descricao, cancellationToken: cancellationToken))
            throw new DescricaoJaCadastradaException();

        _repository.Adicionar(item);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return new ItemEstoqueDto(item.Id, item.Descricao, item.Quantidade);
    }
}