namespace PropostaService.Domain.Entities;

public class OutboxMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Tipo { get; private set; }
    public string Conteudo { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? ProcessadoEm { get; private set; }

    private OutboxMessage() {}

    public OutboxMessage(string type, string content)
    {
        Tipo = type;
        Conteudo = content;
        CriadoEm = DateTime.UtcNow;
    }

    public void DataPocessado()
    {
        ProcessadoEm = DateTime.UtcNow;
    }
}