using MediatR;
using PartsForge.Application.Dtos;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.ItensEstoque.ListarItensEstoque;

public class ListarItensEstoqueQueryHandler(IItemEstoqueRepository repository) : IRequestHandler<ListarItensEstoqueQuery, List<ItemEstoqueDto>>
{
    private readonly IItemEstoqueRepository _repository = repository;

    public async Task<List<ItemEstoqueDto>> Handle(ListarItensEstoqueQuery request, CancellationToken cancellationToken)
    {
        var itens = await _repository.ListarAsync();
        return [.. itens.Select(item => new ItemEstoqueDto(item.Id, item.Descricao, item.Quantidade))];
    }
}