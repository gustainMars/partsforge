
namespace PartsForge.Domain.Exceptions;

public class DecrementoMaiorQueEstoqueException : DomainException
{
    public DecrementoMaiorQueEstoqueException()
        : base("Não é possível decrementar mais do que a quantidade disponível.") { }
}