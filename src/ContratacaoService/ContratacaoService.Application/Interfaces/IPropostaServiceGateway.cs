using ContratacaoService.Application.DTOs;

namespace ContratacaoService.Application.Interfaces;

public interface IPropostaServiceGateway
{
    Task<PropostaStatusDto?> GetPropostaStatusAsync(Guid propostaId);
}