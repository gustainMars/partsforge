namespace PartsForge.Application.Exceptions.ItensEstoque;

public class ItemEstoqueNaoEncontradoException : AppException
{
    public ItemEstoqueNaoEncontradoException() 
        : base("Item estoque não encontrado.") { }
}