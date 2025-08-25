using PropostaService.Domain.Common.Constants;
using PropostaService.Domain.Common.Enum;
using PropostaService.Domain.Common.Wrappers;

namespace PropostaService.Domain.Entities;

public class Proposta
{
    public Guid Id { get; private set; }
    public string NomeCliente { get; private set; }
    public string CpfCliente { get; private set; }
    public decimal ValorSeguro { get; private set; }
    public PropostaStatus Status { get; private set; } 
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }
    
    private Proposta() {}
    
    public static DomainResult<Proposta> Criar(string nomeCliente, string cpfCliente, decimal valorSeguro)
    {
        if (string.IsNullOrWhiteSpace(nomeCliente))
            return DomainResult<Proposta>.CriarResponseErro(MensagensErroDomain.NomeClienteVazio);
        
        if (!CpfValidator.IsValid(cpfCliente)) 
            return DomainResult<Proposta>.CriarResponseErro(MensagensErroDomain.CpfInvalido);
        
        if (valorSeguro <= 0)
            return DomainResult<Proposta>.CriarResponseErro(MensagensErroDomain.ValorSeguroInvalido);
        
        var proposta = new Proposta
        {
            Id = Guid.NewGuid(),
            NomeCliente = nomeCliente,
            CpfCliente = cpfCliente,
            ValorSeguro = valorSeguro,
            Status = PropostaStatus.EmAnalise,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };
        
        return DomainResult<Proposta>.CriarResponseSucesso(proposta);
    }
    
    public DomainResult<Proposta> Aprovar()
    {
        if (Status != PropostaStatus.EmAnalise)
            return DomainResult<Proposta>.CriarResponseErro(MensagensErroDomain.StatusNaoPermitidoParaAprovacao);

        Status = PropostaStatus.Aprovada;
        DataAtualizacao = DateTime.UtcNow;
        
        return DomainResult<Proposta>.CriarResponseSucesso(this);
    }
    
    public DomainResult<Proposta> Rejeitar()
    {
        if (Status != PropostaStatus.EmAnalise)
            return DomainResult<Proposta>.CriarResponseErro(MensagensErroDomain.StatusNaoPermitidoParaRejeicao);

        Status = PropostaStatus.Rejeitada;
        DataAtualizacao = DateTime.UtcNow;
        
        return DomainResult<Proposta>.CriarResponseSucesso(this);
    }
}