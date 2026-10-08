namespace PartsForge.Application.Dtos;

public record ReceitaDto(int Id, string Descricao, List<ReceitaItemDto> Itens);
