namespace PropostaService.Application.DTOs;
public record PropostaAprovadaEvent(
    Guid Id,
    DateTime DataCriacao
);