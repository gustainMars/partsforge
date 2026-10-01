using MediatR;
using PartsForge.Application.Dtos;

namespace PartsForge.Application.UseCases.ItensEstoque.ListarItensEstoque;

public record ListarItensEstoqueQuery : IRequest<List<ItemEstoqueDto>>;
