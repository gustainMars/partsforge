using Moq;
using PartsForge.Application.Exceptions;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.Receitas.ExecutarReceita;
using PartsForge.Domain.Entities;
using PartsForge.Domain.Exceptions;

namespace PartsForge.Application.Tests.UseCases.Receitas.ExecutarReceita;

public class ExecutarReceitaCommandHandlerTest
{
    [Fact]
    public async Task Handle_ReceitaExistenteItensSuficientes_Sucesso()
    {
        var itemEstoque = new ItemEstoque(1, "Item A", 10);
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [receitaItem]);

        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(1))
            .ReturnsAsync(receita);
        
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new ExecutarReceitaCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await handler.Handle(new ExecutarReceitaCommand(1), CancellationToken.None);

        Assert.Equal(5, itemEstoque.Quantidade);
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReceitaNaoEncontrada_DeveLancarReceitaNaoEncontradaException()
    {
        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(1))
            .ReturnsAsync((Receita?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new ExecutarReceitaCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ReceitaNaoEncontradaException>(() => handler.Handle(new ExecutarReceitaCommand(1), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReceitaExistenteItensInsuficientes_DeveLancarReceitaInviavelException()
    {
        var itemEstoque = new ItemEstoque(1, "Item A", 5);
        var receitaItem = new ReceitaItem(1, itemEstoque, 10);
        var receita = new Receita("Receita Teste", [receitaItem]);

        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(1))
            .ReturnsAsync(receita);
        
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new ExecutarReceitaCommandHandler(repositorioMock.Object, unitOfWorkMock.Object);

        await Assert.ThrowsAsync<ReceitaInviavelException>(() => handler.Handle(new ExecutarReceitaCommand(1), CancellationToken.None));
        unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}