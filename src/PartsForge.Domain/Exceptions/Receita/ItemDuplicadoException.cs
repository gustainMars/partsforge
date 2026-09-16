using PartsForge.Domain.Entities;

namespace PartsForge.Domain.Exceptions;

public class ItemDuplicadoException : DomainException
{
    public IReadOnlyList<ReceitaItem> ItensDuplicados { get; }
    
    public ItemDuplicadoException(IReadOnlyList<ReceitaItem> itensDuplicados)
        : base("A receita contém itens duplicados.") 
    {
        ItensDuplicados = itensDuplicados;
    }
}