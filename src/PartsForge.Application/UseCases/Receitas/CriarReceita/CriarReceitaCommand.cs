using MediatR;
using PartsForge.Application.Dtos;

namespace PartsForge.Application.UseCases.Receitas.CriarReceita;

public record CriarReceitaCommand(string Descricao, List<ItemReceitaInput> Itens) : IRequest<ReceitaDto>;
