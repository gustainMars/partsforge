
using PartsForge.Domain.Exceptions;

namespace PartsForge.Domain.Entities;

public class ItemEstoque
{
    public const int TamanhoMaximoDescricao = 200;
    public const int QuantidadeMaxima = int.MaxValue;

    public int Id { get; init; }
    public string Descricao { get; private set; }
    public int Quantidade { get; private set; }

    public ItemEstoque(string descricao, int quantidade)
    {
        Descricao = ValidarENormalizarDescricao(descricao);
        GarantirQuantidadeValida(quantidade);
        Quantidade = quantidade;
    }

    public void AlterarDescricao(string descricao)
    {
        Descricao = ValidarENormalizarDescricao(descricao);
    }

    public void Incrementar(int quantidade)
    {
        if (quantidade < 0)
            throw new IncrementoNegativoException();

        if (quantidade > QuantidadeMaxima - Quantidade)
            throw new EstoqueExcedeLimiteException();

        Quantidade += quantidade;
    }

    public void Decrementar(int quantidade)
    {
        if (quantidade < 0)
            throw new DecrementoNegativoException();

        if (quantidade > Quantidade)
            throw new DecrementoMaiorQueEstoqueException();

        Quantidade -= quantidade;
    }

    private static string ValidarENormalizarDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DescricaoObrigatoriaException();

        descricao = descricao.Trim();

        if (descricao.Length > TamanhoMaximoDescricao)
            throw new DescricaoExcedeTamanhoMaximoException();
        
        return descricao;
    }

    private static void GarantirQuantidadeValida(int quantidade)
    {
        if (quantidade < 0)
            throw new EstoqueNegativoException();
    }
}
