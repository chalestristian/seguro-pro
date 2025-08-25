using MediatR;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;

namespace PropostaService.Application.Features.ListarPropostas;

public record ListarPropostasQuery() : IRequest<ApplicationResult<IEnumerable<PropostaResponse>>>;