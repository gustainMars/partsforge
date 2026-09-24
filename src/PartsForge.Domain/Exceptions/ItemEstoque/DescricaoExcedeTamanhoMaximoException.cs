using PartsForge.Domain.Entities;

namespace PartsForge.Domain.Exceptions;

public class DescricaoExcedeTamanhoMaximoException : DomainException
{
    public DescricaoExcedeTamanhoMaximoException()
        : base($"A descrição do item excede o tamanho máximo permitido de {ItemEstoque.TamanhoMaximoDescricao} caracteres.") { }
}