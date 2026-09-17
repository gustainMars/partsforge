namespace PartsForge.Application.Exceptions;

public class ReceitaNaoEncontradaException : AppException
{
    public ReceitaNaoEncontradaException() 
        : base("Receita não encontrada.") { }
}