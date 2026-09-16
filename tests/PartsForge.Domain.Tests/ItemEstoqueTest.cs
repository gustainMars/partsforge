using PartsForge.Domain.Entities;
using PartsForge.Domain.Exceptions;

namespace PartsForge.Domain.Tests;

public class ItemEstoqueTest
{
    [Fact]
    public void Decrementar_QuantidadeValida_DeveSobrar()
    {
        ItemEstoque itemEstoque = new(1, "Teste", 10);
        itemEstoque.Decrementar(5);
        Assert.Equal(5, itemEstoque.Quantidade);
    }

    [Fact]
    public void Decrementar_QuantidadeExataEmEstoque_NaoDeveSobrar()
    {
        ItemEstoque itemEstoque = new(1, "Teste", 10);
        itemEstoque.Decrementar(10);
        Assert.Equal(0, itemEstoque.Quantidade);
    }

    [Fact]
    public void Decrementar_QuantidadeMaiorQueEstoque_DeveLancarDecrementoMaiorQueEstoqueException()
    {
        ItemEstoque itemEstoque = new(1, "Teste", 10);
        Assert.Throws<DecrementoMaiorQueEstoqueException>(() => itemEstoque.Decrementar(15));
    }

    [Fact]
    public void Decrementar_QuantidadeNegativa_DeveLancarDecrementoNegativoException()
    {
        ItemEstoque itemEstoque = new(1, "Teste", 10);
        Assert.Throws<DecrementoNegativoException>(() => itemEstoque.Decrementar(-5));
    }

    [Fact]
    public void Construtor_QuantidadeNegativa_DeveLancarEstoqueNegativoException()
    {
        Assert.Throws<EstoqueNegativoException>(() => new ItemEstoque(1, "Teste", -10));
    }
}
