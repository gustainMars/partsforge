
using PartsForge.Domain.Exceptions;

namespace PartsForge.Domain.Entities;

public class ReceitaItem
{
    public int ReceitaId { get; set; }
    public int ItemEstoqueId { get; set; }
    public ItemEstoque ItemEstoque { get; }
    public int Quantidade { get; private set; }

    public ReceitaItem(int receitaId, ItemEstoque item, int quantidade)
    {
        ReceitaId = receitaId;
        ItemEstoqueId = item.Id;
        ItemEstoque = item;
        GarantirQuantidadeValida(quantidade);
        Quantidade = quantidade;
    }

    private static void GarantirQuantidadeValida(int quantidade)
    {
        if (quantidade <= 0)
            throw new QuantidadeInvalidaException();
    }
}
