using MediatR;

namespace PartsForge.Application.UseCases.ReceitaItens.AlterarQuantidade;

public record AlterarQuantidadeReceitaItemCommand(int ReceitaId, int ItemEstoqueId, int NovaQuantidade) : IRequest;
