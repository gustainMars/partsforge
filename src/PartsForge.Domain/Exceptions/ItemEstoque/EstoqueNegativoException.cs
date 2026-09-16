
namespace PartsForge.Domain.Exceptions;

public class EstoqueNegativoException : DomainException
{
    public EstoqueNegativoException()
        : base("A quantidade de estoque não pode ser negativa.") { }
}