namespace PartsForge.Application.Exceptions.Receitas;

public class ReceitaNaoEncontradaException : AppException
{
    public ReceitaNaoEncontradaException() 
        : base("Receita não encontrada.") { }
}