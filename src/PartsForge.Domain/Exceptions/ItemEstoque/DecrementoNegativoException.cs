
namespace PartsForge.Domain.Exceptions;

public class DecrementoNegativoException : DomainException
{
    public DecrementoNegativoException()
        : base("A quantidade a ser decrementada não pode ser negativa.") { }
}