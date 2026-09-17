using MediatR;
using PartsForge.Domain.ValueObjects;

namespace PartsForge.Application.UseCases.Receitas.ConsultarViabilidade;

public record ConsultarViabilidadeQuery(int ReceitaId) : IRequest<IReadOnlyList<ItemViabilidade>>;
