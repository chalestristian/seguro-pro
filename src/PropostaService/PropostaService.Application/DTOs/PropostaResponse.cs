using PropostaService.Domain.Common.Enum;

namespace PropostaService.Application.DTOs;

public record PropostaResponse(
    Guid Id,
    decimal ValorSeguro,
    PropostaStatus Status,
    string StatusNome,
    DateTime DataCriacao
);