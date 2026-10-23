using MediatR;
using PartsForge.Application.Dtos;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.UseCases.Receitas.CriarReceita;

public class CriarReceitaCommandHandler(IReceitaRepository receitaRepository, IItemEstoqueRepository itemEstoqueRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CriarReceitaCommand, ReceitaDto>
{
    private readonly IReceitaRepository _receitaRepository = receitaRepository;
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ReceitaDto> Handle(CriarReceitaCommand request, CancellationToken cancellationToken)
    {
        var itensEstoquePorId = new Dictionary<int, ItemEstoque>();
        var idsNaoEncontrados = new List<int>();

        foreach (var itemInput in request.Itens)
        {
            var itemEstoque = await _itemEstoqueRepository.ObterPorIdAsync(itemInput.ItemEstoqueId, cancellationToken);

            if (itemEstoque is null)
                idsNaoEncontrados.Add(itemInput.ItemEstoqueId);
            else
                itensEstoquePorId[itemInput.ItemEstoqueId] = itemEstoque;
        }

        if (idsNaoEncontrados.Count > 0)
            throw new ItensEstoqueNaoEncontradosException(idsNaoEncontrados);

        var receitaItens = request.Itens
            .Select(i => new ReceitaItem(0, itensEstoquePorId[i.ItemEstoqueId], i.Quantidade))
            .ToList();

        var receita = new Receita(request.Descricao, receitaItens);

        _receitaRepository.Adicionar(receita);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return new ReceitaDto(
            receita.Id,
            receita.Descricao,
            [.. receita.Itens.Select(i => new ReceitaItemDto(i.ItemEstoqueId, i.ItemEstoque.Descricao, i.Quantidade))]
        );
    }
}