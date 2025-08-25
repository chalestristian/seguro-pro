using PropostaService.Domain.Common.Enum;

namespace PropostaService.Application.DTOs;

public record PropostaResponse(
    Guid Id,
    string  Nome,
    decimal ValorSeguro,
    PropostaStatus Status,
    string StatusNome,
    DateTime DataCriacao
);