using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartsForge.Application.Dtos;
using PartsForge.Application.UseCases.ReceitaItens.AlterarQuantidade;
using PartsForge.Application.UseCases.ReceitaItens.RemoverReceitaItem;
using PartsForge.Application.UseCases.Receitas.ConsultarViabilidade;
using PartsForge.Application.UseCases.Receitas.CriarReceita;
using PartsForge.Application.UseCases.Receitas.ExecutarReceita;
using PartsForge.Application.UseCases.Receitas.ListarReceitas;
using PartsForge.Application.UseCases.Receitas.ObterReceitaPorId;
using PartsForge.Presentation.Requests;
using PartsForge.Presentation.Responses;

namespace PartsForge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceitasController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReceitaDto>>> Listar(CancellationToken cancellationToken)
    {
        var itens = await _sender.Send(new ListarReceitasQuery(), cancellationToken);
        return Ok(itens);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReceitaDto>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var receita = await _sender.Send(new ObterReceitaPorIdQuery(id), cancellationToken);
        return Ok(receita);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReceitaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReceitaDto>> Criar(CriarReceitaCommand command, CancellationToken cancellationToken)
    {
        var receita = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = receita.Id }, receita);
    }

    [HttpGet("{receitaId}/viabilidade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ItemViabilidadeResponse>>> ConsultarViabilidade(int receitaId, CancellationToken cancellationToken)
    {
        ConsultarViabilidadeQuery query = new(receitaId);
        
        var resultado = await _sender.Send(query, cancellationToken);
        
        return Ok(resultado.Select(ItemViabilidadeResponse.From).ToList());
    }

    [HttpPost("{receitaId}/executar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Executar(int receitaId, CancellationToken cancellationToken)
    {
        ExecutarReceitaCommand command = new(receitaId);
        
        await _sender.Send(command, cancellationToken);
        
        return NoContent();
    }

    [HttpPatch("{receitaId}/itens/{itemEstoqueId}/quantidade")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlterarQuantidadeReceitaItem(int receitaId, int itemEstoqueId, AlterarQuantidadeRequest request, CancellationToken cancellationToken)
    {
        var command = new AlterarQuantidadeReceitaItemCommand(receitaId, itemEstoqueId, request.NovaQuantidade);
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{receitaId}/itens/{itemEstoqueId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverItem(int receitaId, int itemEstoqueId, CancellationToken cancellationToken)
    {
        await _sender.Send(new RemoverReceitaItemCommand(receitaId, itemEstoqueId), cancellationToken);
        return NoContent();
    }
}