using Moq;
using PartsForge.Application.Exceptions.Receitas;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.ReceitaItens.RemoverReceitaItem;
using PartsForge.Domain.Entities;
using PartsForge.Domain.Exceptions;

namespace PartsForge.Application.Tests.UseCases.ReceitaItens.RemoverReceitaItem;

public class RemoverReceitaItemCommandHandlerTest
{
    [Fact]
    public async Task Handle_ReceitaEItemExistentes_DeveRemoverItem()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 2 };
        var receita = new Receita("Receita A",
        [
            new(1, itemEstoque, 5)
        ]) { Id = 1 };

        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(receita.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receita);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new RemoverReceitaItemCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new RemoverReceitaItemCommand(receita.Id, itemEstoque.Id), CancellationToken.None);

        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Empty(receita.Itens);
    }

    [Fact]
    public async Task Handle_ReceitaNaoExistente_DeveLancarReceitaNaoEncontradaException()
    {
        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Receita?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new RemoverReceitaItemCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ReceitaNaoEncontradaException>(() =>
            handler.Handle(new RemoverReceitaItemCommand(1, 2), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ItemNaoExistenteNaReceita_DeveLancarReceitaItemNaoEncontradoException()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 2 };
        var receita = new Receita("Receita A",
        [
            new(1, itemEstoque, 5)
        ]) { Id = 1 };

        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(receita.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receita);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new RemoverReceitaItemCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ReceitaItemNaoEncontradoException>(() =>
            handler.Handle(new RemoverReceitaItemCommand(receita.Id, 999), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}