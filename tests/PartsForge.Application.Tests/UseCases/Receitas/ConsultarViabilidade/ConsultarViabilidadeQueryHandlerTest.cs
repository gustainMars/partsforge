using Moq;
using PartsForge.Application.Exceptions;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.Receitas.ConsultarViabilidade;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.Tests.UseCases.Receitas.ConsultarViabilidade;

public class ConsultarViabilidadeQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReceitaExistente_RetornaViabilidade()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        var receitaItem = new ReceitaItem(1, itemEstoque, 5);
        var receita = new Receita("Receita Teste", [receitaItem]);

        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(1))
            .ReturnsAsync(receita);

        var handler = new ConsultarViabilidadeQueryHandler(repositorioMock.Object);

        var resultado = await handler.Handle(new ConsultarViabilidadeQuery(1), CancellationToken.None);

        Assert.Single(resultado);
        Assert.True(resultado[0].Suficiente);
    }

    [Fact]
    public async Task Handle_ReceitaNaoExistente_DeveLancarReceitaNaoEncontradaException()
    {
        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock
            .Setup(r => r.ObterComItensAsync(1))
            .ReturnsAsync((Receita?)null);

        var handler = new ConsultarViabilidadeQueryHandler(repositorioMock.Object);

        await Assert.ThrowsAsync<ReceitaNaoEncontradaException>(() => handler.Handle(new ConsultarViabilidadeQuery(1), CancellationToken.None));
    }
}