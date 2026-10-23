using PartsForge.Domain.Entities;
using PartsForge.Domain.Exceptions;

namespace PartsForge.Domain.Tests;

public class ReceitaTest
{
    [Fact]
    public void Construtor_CriaReceitaComItensDiferentes_Sucesso()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);

        var itemEstoqueB = new ItemEstoque("Item B", 8) { Id = 2 };
        var receitaItemB = new ReceitaItem(2, itemEstoqueB, 3);
        
        var receita = new Receita("Receita Teste", [ receitaItem, receitaItemB ]);
        Assert.Equal("Receita Teste", receita.Descricao);
        Assert.Equal(2, receita.Itens.Count);
    }

    [Fact]
    public void Construtor_CriaReceitaComItensIguais_DeveLancarItemDuplicadoException()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        
        Assert.Throws<ItemDuplicadoException>(() => new Receita("Receita Teste", [ receitaItem, receitaItem ]));
    }

    [Fact]
    public void AdicionarItem_AdicionaItemNaReceita_Sucesso()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [ receitaItem ]);
        
        var itemEstoqueB = new ItemEstoque("Item B", 8) { Id = 2 };
        var receitaItemB = new ReceitaItem(2, itemEstoqueB, 3);
        
        receita.AdicionarItem(receitaItemB);
        
        Assert.Equal(2, receita.Itens.Count);
    }

    [Fact]
    public void AdicionarItem_AdicionaItemDuplicado_DeveLancarItemDuplicadoException()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [ receitaItem ]);
        
        Assert.Throws<ItemDuplicadoException>(() => receita.AdicionarItem(receitaItem));
    }

    [Fact]
    public void VerificarViabilidade_ItemSuficiente_RetornaItemComSuficienteVerdadeiro()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [receitaItem]);

        var resultado = receita.VerificarViabilidade();

        Assert.Single(resultado);
        Assert.True(resultado[0].Suficiente);
    }
    
    [Fact]
    public void VerificarViabilidade_ItemInsuficiente_RetornaItemComSuficienteFalso()
    {
        var itemEstoque = new ItemEstoque("Item A", 5) { Id = 1 };
        var receitaItem = new ReceitaItem(1, itemEstoque, 10);
        var receita = new Receita("Receita Teste", [receitaItem]);

        var resultado = receita.VerificarViabilidade();

        Assert.Single(resultado);
        Assert.False(resultado[0].Suficiente);
    }

    [Fact]
    public void Executar_CriaReceitaComItensSuficientes_Sucesso()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [ receitaItem ]);

        receita.Executar();
        Assert.Equal(5, itemEstoque.Quantidade);
    }

    [Fact]
    public void Executar_CriaReceitaComItensInsuficientes_DeveLancarReceitaInviavelException()
    {
        var itemEstoque = new ItemEstoque("Item A", 3) { Id = 1 };
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [ receitaItem ]);

        Assert.Throws<ReceitaInviavelException>(() => receita.Executar());
    }

    [Fact]
    public void Executar_UmItemInsuficienteEOutroSuficiente_NaoDecrementaNenhum()
    {
        var itemSuficiente = new ItemEstoque("Item A", 10) { Id = 1 };
        var itemInsuficiente = new ItemEstoque("Item B", 2) { Id = 2 };

        var receitaItemA = new ReceitaItem(1, itemSuficiente, 5);
        var receitaItemB = new ReceitaItem(1, itemInsuficiente, 5);

        var receita = new Receita("Receita Teste", [receitaItemA, receitaItemB]);

        Assert.Throws<ReceitaInviavelException>(() => receita.Executar());
        Assert.Equal(10, itemSuficiente.Quantidade);
    }

    [Fact]
    public void RemoverItem_ItemExistenteComOutrosItensNaReceita_DeveRemoverApenasOItemCorreto()
    {
        var itemA = new ItemEstoque("Item A", 10) { Id = 1 };
        var itemB = new ItemEstoque("Item B", 10) { Id = 2 };
        var receita = new Receita("Receita Teste", [
            new ReceitaItem(1, itemA, 5),
            new ReceitaItem(1, itemB, 3)
        ]);

        receita.RemoverItem(itemA.Id);

        Assert.Single(receita.Itens);
        Assert.Equal(itemB.Id, receita.Itens[0].ItemEstoqueId);
    }

    [Fact]
    public void RemoverItem_ItemInexistente_DeveLancarReceitaItemNaoEncontradoException()
    {
        var itemA = new ItemEstoque("Item A", 10) { Id = 1 };
        var receita = new Receita("Receita Teste", [new ReceitaItem(1, itemA, 5)]);

        Assert.Throws<ReceitaItemNaoEncontradoException>(() => receita.RemoverItem(999));
        Assert.Single(receita.Itens);
    }

    [Fact]
    public void AlterarQuantidadeItem_ItemExistenteComOutrosItensNaReceita_DeveAlterarQuantidadeDeUmItem()
    {
        var quantidadeItemB = 5;
        var itemA = new ItemEstoque("Item A", 10) { Id = 1 };
        var itemB = new ItemEstoque("Item B", 10) { Id = 2 };
        var receita = new Receita("Receita Teste", [
            new ReceitaItem(1, itemA, 5),
            new ReceitaItem(1, itemB, quantidadeItemB)
        ]);
        var novaQuantidade = 9;

        receita.AlterarQuantidadeItem(itemA.Id, novaQuantidade);

        Assert.Equal(novaQuantidade, receita.Itens.First(i => i.ItemEstoqueId == itemA.Id).Quantidade);
        Assert.Equal(quantidadeItemB, receita.Itens.First(i => i.ItemEstoqueId == itemB.Id).Quantidade);
    }

    [Fact]
    public void AlterarQuantidadeItem_ItemInexistente_DeveLancarReceitaItemNaoEncontradoException()
    {
        var itemA = new ItemEstoque("Item A", 10) { Id = 1 };
        var receita = new Receita("Receita Teste", [new ReceitaItem(1, itemA, 5)]);

        Assert.Throws<ReceitaItemNaoEncontradoException>(() => receita.AlterarQuantidadeItem(999, 10));
        Assert.Single(receita.Itens);
    }
}
