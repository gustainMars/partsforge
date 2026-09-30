using MediatR;
using PartsForge.Application.Dtos;

namespace PartsForge.Application.UseCases.ItensEstoque.ObterItemEstoquePorId;

public record ObterItemEstoquePorIdQuery(int Id) : IRequest<ItemEstoqueDto>;