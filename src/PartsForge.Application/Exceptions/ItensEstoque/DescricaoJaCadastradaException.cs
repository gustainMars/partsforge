namespace PartsForge.Application.Exceptions.ItensEstoque;

public class DescricaoJaCadastradaException : AppException
{
    public DescricaoJaCadastradaException() 
        : base("Descrição já cadastrada.") { }
}