using MediatR;

namespace PartsForge.Application.UseCases.ItensEstoque.AdicionarEstoque;

public record AdicionarEstoqueItemEstoqueCommand(int Id, int Quantidade) : IRequest;
