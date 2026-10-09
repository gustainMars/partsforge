using MediatR;
using PartsForge.Application.Dtos;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.ItensEstoque.ObterItemEstoquePorId;

public class ObterItemEstoquePorIdQueryHandler(IItemEstoqueRepository repository) : IRequestHandler<ObterItemEstoquePorIdQuery, ItemEstoqueDto>
{
    private readonly IItemEstoqueRepository _repository = repository;
    
    public async Task<ItemEstoqueDto> Handle(ObterItemEstoquePorIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new ItemEstoqueNaoEncontradoException();
        
        return new ItemEstoqueDto(item.Id, item.Descricao, item.Quantidade);
    }
}