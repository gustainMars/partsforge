using Moq;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.ItensEstoque.CriarItemEstoque;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.Tests.UseCases.ItensEstoque.CriarItemEstoque;

public class CriarItemEstoqueCommandHandlerTest
{
    [Fact]
    public async Task Handle_DescricaoNaoExiste_DeveSuceder()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ExisteComDescricaoAsync(itemEstoque.Descricao, It.IsAny<int?>()))
            .ReturnsAsync(false);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new CriarItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        var resultado = await handler.Handle(new CriarItemEstoqueCommand(itemEstoque.Descricao, itemEstoque.Quantidade), CancellationToken.None);

        repositorioMock.Verify(r => r.Adicionar(It.IsAny<ItemEstoque>()), Times.Once);
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DescricaoExiste_DeveLancarDescricaoJaCadastradaException()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ExisteComDescricaoAsync(itemEstoque.Descricao, It.IsAny<int?>()))
            .ReturnsAsync(true);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new CriarItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<DescricaoJaCadastradaException>(() => handler.Handle(new CriarItemEstoqueCommand(itemEstoque.Descricao, itemEstoque.Quantidade), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}