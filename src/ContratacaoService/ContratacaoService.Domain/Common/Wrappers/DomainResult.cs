namespace ContratacaoService.Domain.Common.Wrappers;

public class DomainResult<T>
{
    public bool Sucesso { get; private set; }
    public string MensagemErro { get; private set; } = null!;
    public T Data { get; private set; } = default!;

    public static DomainResult<T> CriarResponseSucesso(T data) =>
        new() { Sucesso = true,  Data = data };

    public static DomainResult<T> CriarResponseErro(string message) =>
        new() { Sucesso = false, MensagemErro = message};
}