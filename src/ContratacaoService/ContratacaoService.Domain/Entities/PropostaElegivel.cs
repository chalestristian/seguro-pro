namespace ContratacaoService.Domain.Entities;

public class PropostaElegivel
{
    public Guid PropostaId { get; private set; }
    public DateTime AprovadaEm { get; private set; }

    private PropostaElegivel() {}

    public PropostaElegivel(Guid propostaId)
    {
        PropostaId = propostaId;
        AprovadaEm = DateTime.UtcNow;
    }
}