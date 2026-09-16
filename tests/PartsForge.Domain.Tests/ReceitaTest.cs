using PartsForge.Domain.Entities;
using PartsForge.Domain.Exceptions;

namespace PartsForge.Domain.Tests;

public class ReceitaTest
{
    [Fact]
    public void Construtor_CriaReceitaComItensDiferentes_Sucesso()
    {
        var itemEstoque = new ItemEstoque(1, "Item A", 10);
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);

        var itemEstoqueB = new ItemEstoque(2, "Item B", 8);
        var receitaItemB = new ReceitaItem(2, itemEstoqueB, 3);
        
        var receita = new Receita("Receita Teste", [ receitaItem, receitaItemB ]);
        Assert.Equal("Receita Teste", receita.Descricao);
        Assert.Equal(2, receita.Itens.Count);
    }

    [Fact]
    public void Construtor_CriaReceitaComItensIguais_DeveLancarItemDuplicadoException()
    {
        var itemEstoque = new ItemEstoque(1, "Item A", 10);
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        
        Assert.Throws<ItemDuplicadoException>(() => new Receita("Receita Teste", [ receitaItem, receitaItem ]));
    }

    [Fact]
    public void AdicionarItem_AdicionaItemNaReceita_Sucesso()
    {
        var itemEstoque = new ItemEstoque(1, "Item A", 10);
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [ receitaItem ]);
        
        var itemEstoqueB = new ItemEstoque(2, "Item B", 8);
        var receitaItemB = new ReceitaItem(2, itemEstoqueB, 3);
        
        receita.AdicionarItem(receitaItemB);
        
        Assert.Equal(2, receita.Itens.Count);
    }

    [Fact]
    public void AdicionarItem_AdicionaItemDuplicado_DeveLancarItemDuplicadoException()
    {
        var itemEstoque = new ItemEstoque(1, "Item A", 10);
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [ receitaItem ]);
        
        Assert.Throws<ItemDuplicadoException>(() => receita.AdicionarItem(receitaItem));
    }

    [Fact]
    public void Executar_CriaReceitaComItensSuficientes_Sucesso()
    {
        var itemEstoque = new ItemEstoque(1, "Item A", 10);
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [ receitaItem ]);

        receita.Executar();
        Assert.Equal(5, itemEstoque.Quantidade);
    }

    [Fact]
    public void Executar_CriaReceitaComItensInsuficientes_DeveLancarReceitaInviavelException()
    {
        var itemEstoque = new ItemEstoque(1, "Item A", 3);
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [ receitaItem ]);

        Assert.Throws<ReceitaInviavelException>(() => receita.Executar());
    }

    [Fact]
    public void Executar_UmItemInsuficienteEOutroSuficiente_NaoDecrementaNenhum()
    {
        var itemSuficiente = new ItemEstoque(1, "Item A", 10);
        var itemInsuficiente = new ItemEstoque(2, "Item B", 2);

        var receitaItemA = new ReceitaItem(1, itemSuficiente, 5);
        var receitaItemB = new ReceitaItem(1, itemInsuficiente, 5);

        var receita = new Receita("Receita Teste", [receitaItemA, receitaItemB]);

        Assert.Throws<ReceitaInviavelException>(() => receita.Executar());
        Assert.Equal(10, itemSuficiente.Quantidade);
    }

}
