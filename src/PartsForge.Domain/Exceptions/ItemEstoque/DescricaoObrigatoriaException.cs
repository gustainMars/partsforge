namespace PartsForge.Domain.Exceptions;

public class DescricaoObrigatoriaException : DomainException
{
    public DescricaoObrigatoriaException()
        : base("A descrição do item é obrigatória.") { }
}