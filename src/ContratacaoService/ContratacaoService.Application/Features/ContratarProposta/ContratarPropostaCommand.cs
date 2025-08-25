using ContratacaoService.Application.Common.Wrappers;
using ContratacaoService.Application.DTOs;
using MediatR;

namespace ContratacaoService.Application.Features.ContratarProposta;


public record ContratarPropostaCommand(Guid PropostaId) : IRequest<ApplicationResult<ContratacaoResponse>>;