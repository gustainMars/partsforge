using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartsForge.Application.UseCases.Receitas.ConsultarViabilidade;
using PartsForge.Presentation.Responses;

namespace PartsForge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceitasController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("{receitaId}/viabilidade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ItemViabilidadeResponse>>> ConsultarViabilidade(int receitaId, CancellationToken cancellationToken)
    {
        ConsultarViabilidadeQuery query = new(receitaId);
        
        var resultado = await _sender.Send(query, cancellationToken);
        
        return Ok(resultado.Select(ItemViabilidadeResponse.From).ToList());
    }
}