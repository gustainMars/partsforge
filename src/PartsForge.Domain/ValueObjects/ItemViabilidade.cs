namespace PartsForge.Domain.ValueObjects;

public record ItemViabilidade(int ItemEstoqueId, string Descricao, int Necessario, int Disponivel)
{
    public bool Suficiente => Disponivel >= Necessario;
}