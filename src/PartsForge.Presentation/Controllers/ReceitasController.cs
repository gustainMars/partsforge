using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartsForge.Application.Dtos;
using PartsForge.Application.UseCases.Receitas.ConsultarViabilidade;
using PartsForge.Application.UseCases.Receitas.ExecutarReceita;
using PartsForge.Application.UseCases.Receitas.ListarReceitas;
using PartsForge.Application.UseCases.Receitas.ObterReceitaPorId;
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
}