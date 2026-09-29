using MediatR;

namespace PartsForge.Application.UseCases.ItensEstoque.CriarItemEstoque;

public record CriarItemEstoqueCommand(string Descricao, int Quantidade) : IRequest<int>;