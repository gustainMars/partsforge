using Moq;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.ItensEstoque.ObterItemEstoquePorId;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.Tests.UseCases.ItensEstoque.ObterItemEstoquePorId;

public class ObterItemEstoquePorIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_IdExistente_DeveRetornarDto()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id))
            .ReturnsAsync(itemEstoque);


        var handler = new ObterItemEstoquePorIdQueryHandler(repositorioMock.Object);

        var resultado = await handler.Handle(new ObterItemEstoquePorIdQuery(itemEstoque.Id), CancellationToken.None);

        Assert.Equal(itemEstoque.Descricao, resultado.Descricao);
        Assert.Equal(itemEstoque.Quantidade, resultado.Quantidade);
    }

    [Fact]
    public async Task Handle_IdInexistente_DeveLancarItemEstoqueNaoEncontradoException()
    {
        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((ItemEstoque?)null);

        var handler = new ObterItemEstoquePorIdQueryHandler(repositorioMock.Object);

        await Assert.ThrowsAsync<ItemEstoqueNaoEncontradoException>(() => handler.Handle(new ObterItemEstoquePorIdQuery(1), CancellationToken.None));
    }
}