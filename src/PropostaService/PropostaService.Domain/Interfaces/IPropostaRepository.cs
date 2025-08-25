using PropostaService.Domain.Entities;
namespace PropostaService.Domain.Interfaces;

public interface IPropostaRepository
{
    Task CriarAsync(Proposta proposta);
    Task<Proposta?> BuscarPorIdAsync(Guid id);
    Task<IEnumerable<Proposta>> BuscarAsync();
    Task AtualizarAsync(Proposta proposta);
}