using MediatR;
using PartsForge.Application.Dtos;
using PartsForge.Application.Exceptions.Receitas;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.Receitas.ObterReceitaPorId;

public class ObterReceitaPorIdQueryHandler(IReceitaRepository repository) 
    : IRequestHandler<ObterReceitaPorIdQuery, ReceitaDto>
{
    private readonly IReceitaRepository _repository = repository;

    public async Task<ReceitaDto> Handle(ObterReceitaPorIdQuery request, CancellationToken cancellationToken)
    {
        var receita = await _repository.ObterComItensAsync(request.Id, cancellationToken)
            ?? throw new ReceitaNaoEncontradaException();

        return new ReceitaDto(
            receita.Id,
            receita.Descricao,
            [.. receita.Itens.Select(i => new ReceitaItemDto(
                i.ItemEstoqueId,
                i.ItemEstoque.Descricao,
                i.Quantidade
            ))]
        );
    }
}