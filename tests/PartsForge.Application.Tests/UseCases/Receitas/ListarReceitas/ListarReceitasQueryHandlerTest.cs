using Moq;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.Receitas.ListarReceitas;
using PartsForge.Domain.Entities;

namespace PartsForge.Application.Tests.UseCases.Receitas.ListarReceitas;

public class ListarReceitasQueryHandlerTest
{
    [Fact]
    public async Task Handle_Listar_DeveSuceder()
    {
        var itemEstoque = new ItemEstoque("Item A", 10) { Id = 1 };
        var itemEstoque2 = new ItemEstoque("Item B", 20) { Id = 2 };
        var itemEstoque3 = new ItemEstoque("Item C", 30) { Id = 3 };
        var receitas = new List<Receita>
        {
            new("Receita 1",
            [
                new ReceitaItem(1, itemEstoque, 2),
                new ReceitaItem(2, itemEstoque2, 3)
            ]),
            new("Receita 2",
            [
                new ReceitaItem(3, itemEstoque3, 4)
            ])
        };

        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock.Setup(r => r.ListarComItensAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(receitas);

        var handler = new ListarReceitasQueryHandler(repositorioMock.Object);

        var resultado = await handler.Handle(new ListarReceitasQuery(), CancellationToken.None);

        Assert.Equal(2, resultado.Count);
        Assert.Equal(2, resultado[0].Itens.Count);
        Assert.Equal("Receita 1", resultado[0].Descricao);
        Assert.Equal("Receita 2", resultado[1].Descricao);
        Assert.Equal(1, resultado[0].Itens[0].ItemEstoqueId);
        Assert.Equal("Item A", resultado[0].Itens[0].Descricao);
        Assert.Equal(2, resultado[0].Itens[0].Quantidade);
    }

    [Fact]
    public async Task Handle_ListarSemReceitas_DeveRetornarListaVazia()
    {
        var repositorioMock = new Mock<IReceitaRepository>();
        repositorioMock.Setup(r => r.ListarComItensAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new ListarReceitasQueryHandler(repositorioMock.Object);

        var resultado = await handler.Handle(new ListarReceitasQuery(), CancellationToken.None);

        Assert.Empty(resultado);
    }
}