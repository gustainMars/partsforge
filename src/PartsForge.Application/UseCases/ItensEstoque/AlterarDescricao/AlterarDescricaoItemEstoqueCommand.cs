using MediatR;

namespace PartsForge.Application.UseCases.ItensEstoque.AlterarDescricao;

public record AlterarDescricaoItemEstoqueCommand(int Id, string NovaDescricao) : IRequest;