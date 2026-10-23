using MediatR;

namespace PartsForge.Application.UseCases.ReceitaItens.RemoverReceitaItem;

public record RemoverReceitaItemCommand(int ReceitaId, int ItemEstoqueId) : IRequest;