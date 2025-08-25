using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Domain.Interfaces;

public interface IContratacaoRepository{
    Task AdicionarAsync(Contratacao contratacao);

    Task<Contratacao?> BuscarPorIdAsync(Guid id);

    Task<IEnumerable<Contratacao>> ListarTodasAsync();

    public Task<Contratacao?> BuscarPorPropostaIdAsync(Guid propostaId);

}