using Moq;
using PartsForge.Application.Exceptions.Receitas;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.Receitas.ObterReceitaPorId;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.Tests.UseCases.Receitas.ObterReceitaPorId;

public class ObterReceitaPorIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReceitaExistente_DeveRetornarReceita()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        var itemEstoque2 = new ItemEstoque("Item B", 20) { Id = 2 };
        var receita = new Receita("Receita",
        [
            new ReceitaItem(1, itemEstoque, 2),
            new ReceitaItem(2, itemEstoque2, 3)
        ])
        {
            Id = 1
        };

        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(receita.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receita);

        var handler = new ObterReceitaPorIdQueryHandler(repositorioMock.Object);
        var resultado = await handler.Handle(new ObterReceitaPorIdQuery(receita.Id), CancellationToken.None);

        Assert.Equal(receita.Id, resultado.Id);
        Assert.Equal(receita.Descricao, resultado.Descricao);
        Assert.Equal(2, resultado.Itens.Count);
        Assert.Equal("Item A", resultado.Itens[0].Descricao);
        Assert.Equal(2, resultado.Itens[0].Quantidade);
        Assert.Equal("Item B", resultado.Itens[1].Descricao);
        Assert.Equal(3, resultado.Itens[1].Quantidade);
    }

    [Fact]
    public async Task Handle_ReceitaInexistente_DeveLancarReceitaNaoEncontradaException()
    {
        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Receita?)null);

        var handler = new ObterReceitaPorIdQueryHandler(repositorioMock.Object);
        await Assert.ThrowsAsync<ReceitaNaoEncontradaException>(async () => await handler.Handle(new ObterReceitaPorIdQuery(1), CancellationToken.None));
    }
}