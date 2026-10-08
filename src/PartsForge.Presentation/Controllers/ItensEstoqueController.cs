using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartsForge.Application.Dtos;
using PartsForge.Application.UseCases.ItensEstoque.ListarItensEstoque;
using PartsForge.Application.UseCases.ItensEstoque.ObterItemEstoquePorId;
using PartsForge.Application.UseCases.ItensEstoque.CriarItemEstoque;
using PartsForge.Application.UseCases.ItensEstoque.AlterarDescricao;
using PartsForge.Presentation.Requests;
using PartsForge.Application.UseCases.ItensEstoque.AdicionarEstoque;
using PartsForge.Application.UseCases.ItensEstoque.RemoverItemEstoque;

namespace PartsForge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItensEstoqueController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ItemEstoqueDto>>> Listar(CancellationToken cancellationToken)
    {
        var itens = await _sender.Send(new ListarItensEstoqueQuery(), cancellationToken);
        return Ok(itens);
    }

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
    public async Task<ActionResult<ItemEstoqueDto>> Criar(CriarItemEstoqueCommand command, CancellationToken cancellationToken)
    {
        var item = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = item.Id }, item);
    }

    [HttpPatch("{id}/descricao")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AlterarDescricao(int id, AlterarDescricaoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new AlterarDescricaoItemEstoqueCommand(id, request.NovaDescricao), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id}/estoque/entrada")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdicionarEstoque(int id, AdicionarEstoqueRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new AdicionarEstoqueItemEstoqueCommand(id, request.Quantidade), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new RemoverItemEstoqueCommand(id), cancellationToken);
        return NoContent();
    }
}