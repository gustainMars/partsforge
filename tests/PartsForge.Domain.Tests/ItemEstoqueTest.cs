using PartsForge.Domain.Entities;
using PartsForge.Domain.Exceptions;

namespace PartsForge.Domain.Tests;

public class ItemEstoqueTest
{
    [Fact]
    public void Construtor_DescricaoValida_InstanciaCriadaComSucesso()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        Assert.Equal("Teste", itemEstoque.Descricao);
        Assert.Equal(10, itemEstoque.Quantidade);
    }

    [Fact]
    public void Construtor_DescricaoComTamanhoMaximo_InstanciaCriadaComSucesso()
    {
        ItemEstoque itemEstoque = new(new string('A', ItemEstoque.TamanhoMaximoDescricao), 10) { Id = 1 };
        Assert.Equal(new string('A', ItemEstoque.TamanhoMaximoDescricao), itemEstoque.Descricao);
        Assert.Equal(10, itemEstoque.Quantidade);
    }

    [Fact]
    public void Construtor_DescricaoVazia_DeveLancarDescricaoObrigatoriaException()
    {
        Assert.Throws<DescricaoObrigatoriaException>(() => new ItemEstoque("", 10));
    }

    [Fact]
    public void Construtor_DescricaoComEspacosEmBranco_DeveLancarDescricaoObrigatoriaException()
    {
        Assert.Throws<DescricaoObrigatoriaException>(() => new ItemEstoque("   ", 10));
    }

    [Fact]
    public void Construtor_DescricaoExcedeTamanhoMaximo_DeveLancarDescricaoExcedeTamanhoMaximoException()
    {
        Assert.Throws<DescricaoExcedeTamanhoMaximoException>(() => new ItemEstoque(new string('A', ItemEstoque.TamanhoMaximoDescricao + 1), 10));
    }

    [Fact]
    public void Construtor_DescricaoComEspacosNasPontas_DescricaoDeveFicarSemEspacos()
    {
        ItemEstoque itemEstoque = new("   Teste   ", 10) { Id = 1 };
        Assert.Equal("Teste", itemEstoque.Descricao);
    }

    [Fact]
    public void AlterarDescricao_DescricaoValida_DescricaoDaInstanciaDeveSerIgualANova()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        string descricaoNova = "Teste 2";
        itemEstoque.AlterarDescricao(descricaoNova);
        Assert.Equal(descricaoNova, itemEstoque.Descricao);
    }

    [Fact]
    public void AlterarDescricao_DescricaoComEspacosNasPontas_DescricaoDaInstanciaNaoDeveTerEspacosEmBranco()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        string descricaoNova = " Novo ";
        itemEstoque.AlterarDescricao(descricaoNova);
        Assert.Equal(descricaoNova.Trim(), itemEstoque.Descricao);
    }

    [Fact]
    public void AlterarDescricao_DescricaoEmBranco_DeveLancarDescricaoObrigatoriaException()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        string descricaoNova = " ";
        Assert.Throws<DescricaoObrigatoriaException>(() => itemEstoque.AlterarDescricao(descricaoNova));
        Assert.Equal("Teste", itemEstoque.Descricao);
    }

    [Fact]
    public void AlterarDescricao_DescricaoExcedeTamanhoMaximo_DeveLancarDescricaoExcedeTamanhoMaximoException()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        Assert.Throws<DescricaoExcedeTamanhoMaximoException>(() => itemEstoque.AlterarDescricao(new string('A', ItemEstoque.TamanhoMaximoDescricao + 1)));
        Assert.Equal("Teste", itemEstoque.Descricao);
    }

    [Fact]
    public void Incrementar_QuantidadeValida_DeveSobrar()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        itemEstoque.Incrementar(5);
        Assert.Equal(15, itemEstoque.Quantidade);
    }

    [Fact]
    public void Incrementar_QuantidadeNegativa_DeveLancarIncrementoNegativoException()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        Assert.Throws<IncrementoNegativoException>(() => itemEstoque.Incrementar(-5));
    }

    [Fact]
    public void Incrementar_QuantidadeAdicionadaExcedeLimite_DeveLancarEstoqueExcedeLimiteException()
    {
        ItemEstoque itemEstoque = new("Teste", ItemEstoque.QuantidadeMaxima) { Id = 1 };
        Assert.Throws<EstoqueExcedeLimiteException>(() => itemEstoque.Incrementar(1));
    }

    [Fact]
    public void Incrementar_QuantidadeAtingeExatamenteOMaximo_DeveAceitar()
    {
        ItemEstoque itemEstoque = new("Teste", ItemEstoque.QuantidadeMaxima - 5) { Id = 1 };
        itemEstoque.Incrementar(5);
        Assert.Equal(ItemEstoque.QuantidadeMaxima, itemEstoque.Quantidade);
    }

    [Fact]
    public void Decrementar_QuantidadeValida_DeveSobrar()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        itemEstoque.Decrementar(5);
        Assert.Equal(5, itemEstoque.Quantidade);
    }

    [Fact]
    public void Decrementar_QuantidadeExataEmEstoque_NaoDeveSobrar()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        itemEstoque.Decrementar(10);
        Assert.Equal(0, itemEstoque.Quantidade);
    }

    [Fact]
    public void Decrementar_QuantidadeMaiorQueEstoque_DeveLancarDecrementoMaiorQueEstoqueException()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        Assert.Throws<DecrementoMaiorQueEstoqueException>(() => itemEstoque.Decrementar(15));
    }

    [Fact]
    public void Decrementar_QuantidadeNegativa_DeveLancarDecrementoNegativoException()
    {
        ItemEstoque itemEstoque = new("Teste", 10) { Id = 1 };
        Assert.Throws<DecrementoNegativoException>(() => itemEstoque.Decrementar(-5));
    }

    [Fact]
    public void Construtor_QuantidadeNegativa_DeveLancarEstoqueNegativoException()
    {
        Assert.Throws<EstoqueNegativoException>(() => new ItemEstoque("Teste", -10));
    }
}
