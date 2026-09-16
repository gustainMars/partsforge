
namespace PartsForge.Domain.Exceptions;

public class DecrementoMaiorQueEstoqueException : ItemEstoqueException
{
    public DecrementoMaiorQueEstoqueException()
        : base("Não é possível decrementar mais do que a quantidade disponível.") { }
}