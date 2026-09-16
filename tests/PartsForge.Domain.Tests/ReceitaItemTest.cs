using PartsForge.Domain.Entities;
using PartsForge.Domain.Exceptions;

namespace PartsForge.Domain.Tests;

public class ReceitaItemTest
{
    [Fact]
    public void Construtor_QuantidadeValida_InstanciaCriadaComSucesso()
    {
        ItemEstoque itemEstoque = new(10, "Item Teste", 20);
        ReceitaItem receitaItem = new(1, itemEstoque, 5);
        Assert.Equal(1, receitaItem.ReceitaId);
        Assert.Equal(10, receitaItem.ItemEstoqueId);
        Assert.Equal(5, receitaItem.Quantidade);
    }

    [Fact]
    public void Construtor_QuantidadeInvalida_DeveLancarQuantidadeInvalidaException()
    {
        ItemEstoque itemEstoque = new(10, "Item Teste", 20);
        Assert.Throws<QuantidadeInvalidaException>(() => new ReceitaItem(1, itemEstoque, 0));
    }
}
