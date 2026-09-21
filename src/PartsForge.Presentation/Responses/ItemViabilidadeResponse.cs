using PartsForge.Domain.ValueObjects;

namespace PartsForge.Presentation.Responses;

public record ItemViabilidadeResponse(int ItemEstoqueId, string Descricao, int Necessario, int Disponivel, bool Suficiente)
{
    public string Disponibilidade => $"{Disponivel}/{Necessario}";

    public static ItemViabilidadeResponse From(ItemViabilidade item) => new(
        item.ItemEstoqueId,
        item.Descricao,
        item.Necessario,
        item.Disponivel,
        item.Suficiente);
}