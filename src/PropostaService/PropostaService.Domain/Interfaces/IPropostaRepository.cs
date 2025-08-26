using PropostaService.Domain.Entities;
namespace PropostaService.Domain.Interfaces;

public interface IPropostaRepository
{
    Task CriarAsync(Proposta proposta);
    Task<Proposta?> BuscarPorIdAsync(Guid id);
    Task<Proposta?> BuscarPeloCpfAsync(string cpf);
    Task<IEnumerable<Proposta>>? BuscarAsync();
    Task AtualizarAsync(Proposta proposta);
    Task AtualizarComOutboxAsync(Proposta proposta, OutboxMessage outboxMessage);
}