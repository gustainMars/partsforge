using PartsForge.Domain.Exceptions;
using PartsForge.Domain.ValueObjects;

namespace PartsForge.Domain.Entities;

public class Receita(string descricao, List<ReceitaItem>? itens)
{
    public int Id { get; set; }
    public string Descricao { get; init; } = descricao;
    public List<ReceitaItem> Itens { get; private set; } = ValidarItens(itens ?? []);

    public void AdicionarItem(ReceitaItem item)
    {
        Itens = ValidarItens([ ..Itens, item ]);
    }

    private static List<ReceitaItem> ValidarItens(List<ReceitaItem> itens)
    {
        var duplicados = itens
            .GroupBy(i => i.ItemEstoqueId)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        if (duplicados.Count > 0)
            throw new ItemDuplicadoException(duplicados);

        return itens;
    }

    public List<ItemViabilidade> VerificarViabilidade()
    {
        return [.. Itens.Select(i => new ItemViabilidade(
            i.ItemEstoqueId,
            i.ItemEstoque.Descricao,
            i.Quantidade,
            i.ItemEstoque.Quantidade
        ))];
    }

    public void Executar()
    {
        var viabilidade = VerificarViabilidade();

        if (viabilidade.Any(iv => !iv.Suficiente))
            throw new ReceitaInviavelException(viabilidade);

        foreach (var item in Itens)
            item.ItemEstoque.Decrementar(item.Quantidade);
    }
}