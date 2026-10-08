using MediatR;

namespace PartsForge.Application.UseCases.ItensEstoque.RemoverItemEstoque;

public record RemoverItemEstoqueCommand(int Id) : IRequest;