
using PartsForge.Domain.Exceptions;

namespace PartsForge.Domain;

public class ItemEstoque
{
    public int Id { get; set; }
    public string Descricao { get; init; }
    public int Quantidade { get; private set; }

    public ItemEstoque(int id, string descricao, int quantidade)
    {
        Id = id;
        Descricao = descricao;
        
        GarantirQuantidadeValida(quantidade);
        Quantidade = quantidade;
    }

    private static void GarantirQuantidadeValida(int quantidade)
    {
        if (quantidade < 0)
            throw new EstoqueNegativoException();
    }

    public void Decrementar(int quantidade)
    {
        if (quantidade < 0)
            throw new DecrementoNegativoException();

        if (quantidade > Quantidade)
            throw new DecrementoMaiorQueEstoqueException();

        Quantidade -= quantidade;
    }
}
