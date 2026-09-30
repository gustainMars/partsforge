using MediatR;
using PartsForge.Application.Dtos;

namespace PartsForge.Application.UseCases.ItensEstoque.CriarItemEstoque;

public record CriarItemEstoqueCommand(string Descricao, int Quantidade) : IRequest<ItemEstoqueDto>;