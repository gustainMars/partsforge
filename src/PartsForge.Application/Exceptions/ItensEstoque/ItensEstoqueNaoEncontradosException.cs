namespace PartsForge.Application.Exceptions.ItensEstoque;

public class ItensEstoqueNaoEncontradosException(IReadOnlyList<int> itemEstoqueIdsNaoEncontrados) 
    : AppException("Um ou mais itens de estoques referneciadosnão foram encontrados.")
{
    public IReadOnlyList<int> ItemEstoqueIdsNaoEncontrados { get; } = itemEstoqueIdsNaoEncontrados;
}