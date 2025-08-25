using ContratacaoService.Domain.Common.Constants;
using ContratacaoService.Domain.Common.Wrappers;

namespace ContratacaoService.Domain.Entities;

public class Contratacao
{
    public Guid Id { get; private set; }
    public Guid PropostaId { get; private set; }
    public DateTime DataContratacao { get; private set; }
    
    private Contratacao() {}
    
    private Contratacao(Guid propostaId)
    {
        Id = Guid.NewGuid();
        PropostaId = propostaId;
        DataContratacao = DateTime.UtcNow;
    }

    public static DomainResult<Contratacao> Contratar(Guid propostaId)
    {
        if (propostaId == Guid.Empty)
            return DomainResult<Contratacao>.CriarResponseErro(MensagensErroDomain.PropostaIdVazio);
        
        return DomainResult<Contratacao>.CriarResponseSucesso(new Contratacao(propostaId));
    }
}