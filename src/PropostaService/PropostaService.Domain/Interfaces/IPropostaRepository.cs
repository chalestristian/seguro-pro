using PropostaService.Domain.Entities;
namespace PropostaService.Domain.Interfaces;

public interface IPropostaRepository
{
    Task CriarAsync(Proposta proposta);
    Task<Proposta?> BuscaPorIdAsync(Guid id);
    Task<IEnumerable<Proposta>> BuscaAsync();
    Task AprovarAsync(Guid id);
    Task ReprovarAsync(Guid id);
}