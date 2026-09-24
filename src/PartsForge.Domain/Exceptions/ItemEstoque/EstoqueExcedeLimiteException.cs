namespace PartsForge.Domain.Exceptions;

public class EstoqueExcedeLimiteException : DomainException
{
    public EstoqueExcedeLimiteException()
        : base("A quantidade informada excede o limite permitido.") { }
}