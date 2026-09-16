
namespace PartsForge.Domain.Exceptions;

public class EstoqueNegativoException : ItemEstoqueException
{
    public EstoqueNegativoException()
        : base("A quantidade de estoque não pode ser negativa.") { }
}