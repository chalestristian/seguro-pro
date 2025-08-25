using PropostaService.Domain.Common.Enum;

namespace PropostaService.Application.DTOs;

public record PropostaResponse(
    Guid Id,
    string  NomeCliente,
    decimal ValorSeguro,
    PropostaStatus StatusProposta,
    string NomeStatusProposta,
    DateTime DataCriacao
);