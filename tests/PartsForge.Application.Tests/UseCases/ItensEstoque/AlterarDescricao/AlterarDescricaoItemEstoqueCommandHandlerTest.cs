using Moq;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.ItensEstoque.AlterarDescricao;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.Tests.UseCases.ItensEstoque.AlterarDescricao;

public class AlterarDescricaoItemEstoqueCommandHandlerTest
{
    [Fact]
    public async Task Handle_DescricaoNaoExiste_DeveSuceder()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        string novaDescricao = "Nova Descrição";

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);
        repositorioMock
            .Setup(r => r.ExisteComDescricaoAsync(novaDescricao, itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AlterarDescricaoItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new AlterarDescricaoItemEstoqueCommand(itemEstoque.Id, novaDescricao), CancellationToken.None);

        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(novaDescricao, itemEstoque.Descricao);
    }

    [Fact]
    public async Task Handle_DescricaoIgualMudandoCaixa_DeveSuceder()
    {
        var itemEstoque = new ItemEstoque("Item a", 10) { Id = 1 };
        string novaDescricao = "Item A";

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);
        repositorioMock
            .Setup(r => r.ExisteComDescricaoAsync(novaDescricao, itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AlterarDescricaoItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new AlterarDescricaoItemEstoqueCommand(itemEstoque.Id, novaDescricao), CancellationToken.None);

        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(novaDescricao, itemEstoque.Descricao);
    }

    [Fact]
    public async Task Handle_DescricaoComEspacosNasPontas_DeveChecarDuplicidadeComValorNormalizado()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        string novaDescricaoComEspacos = "  Nova Descrição  ";
        string novaDescricao = "Nova Descrição";

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);
        repositorioMock
            .Setup(r => r.ExisteComDescricaoAsync(novaDescricao, itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AlterarDescricaoItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new AlterarDescricaoItemEstoqueCommand(itemEstoque.Id, novaDescricaoComEspacos), CancellationToken.None);

        repositorioMock.Verify(r => r.ExisteComDescricaoAsync(novaDescricao, itemEstoque.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ItemNaoEncontrado_DeveLancarItemEstoqueNaoEncontradoException()
    {
        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemEstoque?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AlterarDescricaoItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ItemEstoqueNaoEncontradoException>(() => handler.Handle(new AlterarDescricaoItemEstoqueCommand(1, "Nova Descrição"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DescricaoJaExiste_DeveLancarDescricaoJaCadastradaException()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        string novaDescricao = "Descrição Existente";

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);
        repositorioMock
            .Setup(r => r.ExisteComDescricaoAsync(novaDescricao, itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new AlterarDescricaoItemEstoqueCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<DescricaoJaCadastradaException>(() => handler.Handle(new AlterarDescricaoItemEstoqueCommand(itemEstoque.Id, novaDescricao), CancellationToken.None));
    }
}