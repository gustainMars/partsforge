using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PartsForge.Application.Exceptions;
using PartsForge.Domain.Exceptions;
using PartsForge.Presentation.Responses;

namespace PartsForge.Presentation.ExceptionHandling;

public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            ReceitaNaoEncontradaException => new ProblemDetails { Status = 404, Title = exception.Message },
            ReceitaInviavelException inviavel => new ProblemDetails 
            { 
                Status = 422, 
                Title = exception.Message, 
                Extensions = { ["itens"] = inviavel.Itens.Select(ItemViabilidadeResponse.From).ToList() }
            },
            DomainException => new ProblemDetails { Status = 400, Title = exception.Message },
            _ => null
        };

        if (problem is null) return false;

        httpContext.Response.StatusCode = problem.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}