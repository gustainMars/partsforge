using PartsForge.Domain.ValueObjects;

namespace PartsForge.Domain.Exceptions;

public class ReceitaInviavelException : DomainException
{
    public IReadOnlyList<ItemViabilidade> Itens { get; }
    
    public ReceitaInviavelException(IReadOnlyList<ItemViabilidade> itens)
        : base("A receita não pode ser executada: um ou mais itens têm estoque insuficiente.") 
    {
        Itens = itens;
    }
}