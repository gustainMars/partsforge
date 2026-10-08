using MediatR;
using PartsForge.Application.Dtos;

namespace PartsForge.Application.UseCases.Receitas.ObterReceitaPorId;

public record ObterReceitaPorIdQuery(int Id) : IRequest<ReceitaDto>;
