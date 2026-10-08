using MediatR;
using PartsForge.Application.Dtos;

namespace PartsForge.Application.UseCases.Receitas.ListarReceitas;

public record ListarReceitasQuery : IRequest<List<ReceitaDto>>;
