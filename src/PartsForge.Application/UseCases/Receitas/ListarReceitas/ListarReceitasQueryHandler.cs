using MediatR;
using PartsForge.Application.Dtos;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.Receitas.ListarReceitas;

public class ListarReceitasQueryHandler(IReceitaRepository receitaRepository) : IRequestHandler<ListarReceitasQuery, List<ReceitaDto>>
{
    private readonly IReceitaRepository _receitaRepository = receitaRepository;
    
    public async Task<List<ReceitaDto>> Handle(ListarReceitasQuery request, CancellationToken cancellationToken)
    {
        var receitas = await _receitaRepository.ListarComItensAsync(cancellationToken);
        
        return [.. receitas.Select(r => new ReceitaDto(
            r.Id,
            r.Descricao,
            [.. r.Itens.Select(i => new ReceitaItemDto(
                i.ItemEstoqueId,
                i.ItemEstoque.Descricao,
                i.Quantidade
            ))]
        ))];
    }
}