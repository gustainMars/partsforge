using MediatR;
using PartsForge.Application.Exceptions.ItensEstoque;
using PartsForge.Application.Interfaces;

namespace PartsForge.Application.UseCases.ItensEstoque.AlterarDescricao;

public class AlterarDescricaoItemEstoqueCommandHandler(IItemEstoqueRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<AlterarDescricaoItemEstoqueCommand>
{
    private readonly IItemEstoqueRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(AlterarDescricaoItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.ObterPorIdAsync(request.Id) 
            ?? throw new ItemEstoqueNaoEncontradoException();
        
        item.AlterarDescricao(request.NovaDescricao);

        if (await _repository.ExisteComDescricaoAsync(item.Descricao, item.Id))
            throw new DescricaoJaCadastradaException();
        
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }
}