namespace PartsForge.Domain.Exceptions;

public class ReceitaItemNaoEncontradoException : DomainException
{
    public ReceitaItemNaoEncontradoException()
        : base("O item da receita não foi encontrado.") { }
}