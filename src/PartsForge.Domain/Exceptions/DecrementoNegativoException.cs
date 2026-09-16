
namespace PartsForge.Domain.Exceptions;

public class DecrementoNegativoException : ItemEstoqueException
{
    public DecrementoNegativoException()
        : base("A quantidade a ser decrementada não pode ser negativa.") { }
}