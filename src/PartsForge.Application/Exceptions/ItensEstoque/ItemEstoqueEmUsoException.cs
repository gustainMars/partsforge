namespace PartsForge.Application.Exceptions.ItensEstoque;

public class ItemEstoqueEmUsoException : AppException
{
    public ItemEstoqueEmUsoException() 
        : base("Item estoque em uso.") { }
}