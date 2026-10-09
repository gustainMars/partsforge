using Moq;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.ItensEstoque.ListarItensEstoque;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.Tests.UseCases.ItensEstoque.ListarItensEstoque;

public class ListarItensEstoqueQueryHandlerTest
{
    [Fact]
    public async Task Handle_PossuiItens_DeveRetornarDtos()
    {
        var itemEstoque1 = new ItemEstoque("Item A", 10) { Id = 1 };
        var itemEstoque2 = new ItemEstoque("Item B", 20) { Id = 2 };

        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([itemEstoque1, itemEstoque2]);

        var handler = new ListarItensEstoqueQueryHandler(repositorioMock.Object);

        var resultado = await handler.Handle(new ListarItensEstoqueQuery(), CancellationToken.None);

        Assert.Equal(itemEstoque1.Descricao, resultado[0].Descricao);
        Assert.Equal(itemEstoque1.Quantidade, resultado[0].Quantidade);
        Assert.Equal(itemEstoque2.Descricao, resultado[1].Descricao);
        Assert.Equal(itemEstoque2.Quantidade, resultado[1].Quantidade);
    }

    [Fact]
    public async Task Handle_ListaVazia_DeveRetornarListaVazia()
    {
        var repositorioMock = new Mock<IItemEstoqueRepository>();
        repositorioMock
            .Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new ListarItensEstoqueQueryHandler(repositorioMock.Object);

        var resultado = await handler.Handle(new ListarItensEstoqueQuery(), CancellationToken.None);

        Assert.Empty(resultado);
    }
}