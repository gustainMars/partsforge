namespace PartsForge.Domain.Exceptions;

public class IncrementoNegativoException : DomainException
{
    public IncrementoNegativoException()
        : base("A quantidade a ser incrementada não pode ser negativa.") { }
}