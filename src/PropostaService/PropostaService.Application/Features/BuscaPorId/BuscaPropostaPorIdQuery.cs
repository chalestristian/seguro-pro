using MediatR;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;

namespace PropostaService.Application.Features.BuscaPorId;
public record BuscaPropostaPorIdQuery(Guid id) : IRequest<ApplicationResult<PropostaResponse>>;