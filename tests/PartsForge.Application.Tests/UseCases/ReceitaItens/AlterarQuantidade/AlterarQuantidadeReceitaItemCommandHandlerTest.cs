using Moq;
using PartsForge.Application.Exceptions.Receitas;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.ReceitaItens.AlterarQuantidade;
using PartsForge.Domain.Entities;
using PartsForge.Domain.Exceptions;

namespace PartsForge.Application.Tests.UseCases.ReceitaItens.AlterarQuantidade;

public class AlterarQuantidadeReceitaItemCommandHandlerTest
{
    [Fact]
    public async Task Handle_ReceitaEItemExistentes_DeveAlterarQuantidade()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 2 };
        var receita = new Receita("Receita A",
        [
            new(1, itemEstoque, 5)
        ]) { Id = 1 };
        int novaQuantidade = 10;

        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(receita.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receita);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AlterarQuantidadeReceitaItemCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new AlterarQuantidadeReceitaItemCommand(receita.Id, itemEstoque.Id, novaQuantidade), CancellationToken.None);

        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(novaQuantidade, receita.Itens[0].Quantidade);
    }

    [Fact]
    public async Task Handle_ReceitaNaoExistente_DeveLancarExcecao()
    {
        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Receita?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AlterarQuantidadeReceitaItemCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ReceitaNaoEncontradaException>(() =>
            handler.Handle(new AlterarQuantidadeReceitaItemCommand(1, 2, 10), CancellationToken.None));
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

        var handler = new AlterarQuantidadeReceitaItemCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ReceitaItemNaoEncontradoException>(() =>
            handler.Handle(new AlterarQuantidadeReceitaItemCommand(receita.Id, 999, 10), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuantidadeNegativa_DeveLancarQuantidadeInvalidaException()
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

        var handler = new AlterarQuantidadeReceitaItemCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<QuantidadeInvalidaException>(() =>
            handler.Handle(new AlterarQuantidadeReceitaItemCommand(receita.Id, itemEstoque.Id, -5), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuantidadeZero_DeveLancarQuantidadeInvalidaException()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 2 };
        var receita = new Receita("Receita A",
        [
            new(1, itemEstoque, 5)
        ]) { Id = 1 };
        int novaQuantidade = 0;

        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(receita.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receita);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AlterarQuantidadeReceitaItemCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<QuantidadeInvalidaException>(() =>
            handler.Handle(new AlterarQuantidadeReceitaItemCommand(receita.Id, itemEstoque.Id, novaQuantidade), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}