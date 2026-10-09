using Moq;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.ItensEstoque.RemoverItemEstoque;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.Tests.UseCases.ItensEstoque.RemoverItemEstoque;

public class RemoverItemEstoqueCommandHandlerTest
{
    [Fact]
    public async Task Handle_ItemEstoqueExistenteENaoEstaEmUso_DeveSuceder()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);
        repositorioMock
            .Setup(r => r.EstaEmUsoAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new RemoverItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new RemoverItemEstoqueCommand(itemEstoque.Id), CancellationToken.None);

        repositorioMock.Verify(r => r.Remover(itemEstoque), Times.Once);
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ItemInexistente_DeveLancarItemEstoqueNaoEncontradoException()
    {
        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemEstoque?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new RemoverItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ItemEstoqueNaoEncontradoException>(() => handler.Handle(new RemoverItemEstoqueCommand(1), CancellationToken.None));

        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ItemEstoqueExistenteMasEstaEmUso_DeveLancarItemEstoqueEmUsoException()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);
        repositorioMock
            .Setup(r => r.EstaEmUsoAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new RemoverItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ItemEstoqueEmUsoException>(() => handler.Handle(new RemoverItemEstoqueCommand(itemEstoque.Id), CancellationToken.None));

        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}