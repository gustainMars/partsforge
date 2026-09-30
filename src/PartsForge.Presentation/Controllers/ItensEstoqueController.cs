using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartsForge.Application.Dtos;
using PartsForge.Application.UseCases.ItensEstoque.CriarItemEstoque;
using PartsForge.Application.UseCases.ItensEstoque.ObterItemEstoquePorId;
using PartsForge.Domain.Entities;

namespace PartsForge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItensEstoqueController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemEstoqueDto>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var item = await _sender.Send(new ObterItemEstoquePorIdQuery(id), cancellationToken);
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<int>> Criar(CriarItemEstoqueCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }
}