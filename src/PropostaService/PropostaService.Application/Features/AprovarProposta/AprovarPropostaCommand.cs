using MediatR;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;

namespace PropostaService.Application.Features.AprovarProposta;

public record AprovarPropostaCommand(Guid id) : IRequest<ApplicationResult<PropostaResponse>>;