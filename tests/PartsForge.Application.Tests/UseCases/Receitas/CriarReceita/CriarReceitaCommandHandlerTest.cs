using Moq;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.Receitas.CriarReceita;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.Tests.UseCases.Receitas.CriarReceita;

public class CriarReceitaCommandHandlerTest
{
    [Fact]
    public async Task Handle_ReceitaEItensValidos_DeveSuceder()
    {
        var itemEstoqueA = new ItemEstoque("Item A", 5) { Id = 1 };
        var itemEstoqueB = new ItemEstoque("Item B", 5) { Id = 2 };
        
        List<ItemReceitaInput> receitaItensInput = [
            new ItemReceitaInput(itemEstoqueA.Id, 3),
            new ItemReceitaInput(itemEstoqueB.Id, 2)
        ];

        var itemEstoqueRepositorioMock = new Mock<IItemEstoqueRepository>();
        
        itemEstoqueRepositorioMock.Setup(r => r.ObterPorIdAsync(itemEstoqueA.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoqueA);
        itemEstoqueRepositorioMock.Setup(r => r.ObterPorIdAsync(itemEstoqueB.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoqueB);
        
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var receitaRepositorioMock = new Mock<IReceitaRepository>();
        
        var handler = new CriarReceitaCommandHandler(receitaRepositorioMock.Object, itemEstoqueRepositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new CriarReceitaCommand("Receita teste", receitaItensInput), CancellationToken.None);

        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UmItemEstoqueNaoEncontrado_DeveLancarItensEstoqueNaoEncontradosException()
    {
        var itemEstoque = new ItemEstoque("Item B", 5) { Id = 2 };
        List<ItemReceitaInput> receitaItensInput = [
            new ItemReceitaInput(1, 3),
            new ItemReceitaInput(itemEstoque.Id, 2)
        ];

        var itemEstoqueRepositorioMock = new Mock<IItemEstoqueRepository>();
        itemEstoqueRepositorioMock
            .Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemEstoque?)null);
        
        itemEstoqueRepositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);

        var receitaRepositorioMock = new Mock<IReceitaRepository>();

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new CriarReceitaCommandHandler(receitaRepositorioMock.Object, itemEstoqueRepositorioMock.Object, unitOfWorkMock.Object);

        var excecao = await Assert.ThrowsAsync<ItensEstoqueNaoEncontradosException>(() => handler.Handle(new CriarReceitaCommand("Receita teste", receitaItensInput), CancellationToken.None));
        Assert.Single(excecao.ItemEstoqueIdsNaoEncontrados);
        Assert.Contains(1, excecao.ItemEstoqueIdsNaoEncontrados);
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DoisItensEstoqueNaoEncontrados_DeveLancarItensEstoqueNaoEncontradosException()
    {
        var itemEstoque = new ItemEstoque("Item B", 5) { Id = 2 };
        List<ItemReceitaInput> receitaItensInput = [
            new ItemReceitaInput(1, 3),
            new ItemReceitaInput(3, 3),
            new ItemReceitaInput(itemEstoque.Id, 2)
        ];

        var itemEstoqueRepositorioMock = new Mock<IItemEstoqueRepository>();
        itemEstoqueRepositorioMock
            .Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemEstoque?)null);
        
        itemEstoqueRepositorioMock
            .Setup(r => r.ObterPorIdAsync(itemEstoque.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);

        itemEstoqueRepositorioMock
            .Setup(r => r.ObterPorIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemEstoque?)null);

        var receitaRepositorioMock = new Mock<IReceitaRepository>();

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new CriarReceitaCommandHandler(receitaRepositorioMock.Object, itemEstoqueRepositorioMock.Object, unitOfWorkMock.Object);

        var excecao = await Assert.ThrowsAsync<ItensEstoqueNaoEncontradosException>(() => handler.Handle(new CriarReceitaCommand("Receita teste", receitaItensInput), CancellationToken.None));
        Assert.Equal(2, excecao.ItemEstoqueIdsNaoEncontrados.Count);
        Assert.Contains(1, excecao.ItemEstoqueIdsNaoEncontrados);
        Assert.Contains(3, excecao.ItemEstoqueIdsNaoEncontrados);
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
