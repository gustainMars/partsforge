using MediatR;

namespace PartsForge.Application.UseCases.Receitas.ExecutarReceita;

public record ExecutarReceitaCommand(int ReceitaId) : IRequest<Unit>;