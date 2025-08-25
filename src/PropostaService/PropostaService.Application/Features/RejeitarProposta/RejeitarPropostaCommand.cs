using MediatR;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;

namespace PropostaService.Application.Features.RejeitarProposta;

public record RejeitarPropostaCommand(Guid id) : IRequest<ApplicationResult<PropostaResponse>>;