using Moq;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.ItensEstoque.AdicionarEstoque;
using PartsForge.Domain.Entities;
using PartsForge.Domain.Exceptions;

namespace PartsForge.Application.Tests.UseCases.ItensEstoque.AdicionarEstoque;

public class AdicionarEstoqueItemEstoqueCommandHandlerTest
{
    [Fact]
    public async Task Handle_AdicionarEstoque_DeveSuceder()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        int quantidade = 5;

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);
        
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AdicionarEstoqueItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new AdicionarEstoqueItemEstoqueCommand(itemEstoque.Id, quantidade), CancellationToken.None);

        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(15, itemEstoque.Quantidade);
    }

    [Fact]
    public async Task Handle_ItemNaoEncontrado_DeveLancarItemEstoqueNaoEncontradoException()
    {
        var itemEstoque = new ItemEstoque("Item a", 10) { Id = 1 };

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemEstoque?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AdicionarEstoqueItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ItemEstoqueNaoEncontradoException>(() => handler.Handle(new AdicionarEstoqueItemEstoqueCommand(1, 5), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuantidadeNegativa_DeveLancarIncrementoNegativoException()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        int quantidade = -5;

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AdicionarEstoqueItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<IncrementoNegativoException>(() => handler.Handle(new AdicionarEstoqueItemEstoqueCommand(itemEstoque.Id, quantidade), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuantidadeNoLimite_DeveSuceder()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        int quantidade = ItemEstoque.QuantidadeMaxima - itemEstoque.Quantidade;

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AdicionarEstoqueItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new AdicionarEstoqueItemEstoqueCommand(itemEstoque.Id, quantidade), CancellationToken.None);

        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(ItemEstoque.QuantidadeMaxima, itemEstoque.Quantidade);
    }

    [Fact]
    public async Task Handle_QuantidadeMaximaExcedida_DeveLancarEstoqueExcedeLimiteException()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        int quantidade = ItemEstoque.QuantidadeMaxima - itemEstoque.Quantidade + 1;

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AdicionarEstoqueItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<EstoqueExcedeLimiteException>(() => handler.Handle(new AdicionarEstoqueItemEstoqueCommand(itemEstoque.Id, quantidade), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}