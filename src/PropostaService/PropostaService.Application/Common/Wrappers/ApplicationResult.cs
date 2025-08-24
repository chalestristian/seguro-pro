namespace PropostaService.Application.Common.Wrappers;

public class ApplicationResult<T>
{
    private static readonly string MensagemSucesso = "A requisição foi realizada com sucesso.";

    public bool Sucesso { get; private set; }
    public int StatusCode { get; private set; }
    public string? Mensagem { get; private set; }
    public T Data { get; private set; } = default!;
    public string? Erro { get; private set; }

    private ApplicationResult() { }

    public static ApplicationResult<T> CriarResponseSucesso(T data, int statusCode)
    {
        return new ApplicationResult<T>
        {
            Sucesso = true,
            StatusCode = statusCode,
            Mensagem = MensagemSucesso,
            Data = data,
            Erro = null!
        };
    }

    public static ApplicationResult<T> CriarResponseErro(string message, int statusCode)
    {
        return new ApplicationResult<T>
        {
            Sucesso = false,
            StatusCode = statusCode,
            Mensagem = message,
            Data = default!,
        };
    }
}