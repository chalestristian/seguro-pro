using MediatR;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;

namespace PropostaService.Application.Features.CriarProposta;

public record CriarPropostaCommand(string NomeCliente, string CpfCliente, decimal ValorSeguro) : IRequest<ApplicationResult<PropostaResponse>>;