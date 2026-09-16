namespace PartsForge.Domain.Exceptions;

public class QuantidadeInvalidaException : DomainException
{
    public QuantidadeInvalidaException()
        : base("A quantidade deve ser maior que zero.") { }
}