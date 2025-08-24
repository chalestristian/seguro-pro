namespace PropostaService.Domain.Common.Constants;

public static class MensagensErroDomain
{
    public static readonly string NomeClienteVazio = "Nome do cliente não pode ser vazio.";
    public static readonly string CpfInvalido = "O cpf informado não é valido.";
    public static readonly string ValorSeguroInvalido = "O valor do seguro não é valido.";
    public static readonly string StatusNaoPermitidoParaAprovacao = "Apenas propostas com o status 'Em Análise' podem ser aprovadas.";
    public static readonly string StatusNaoPermitidoParaReprovacao = "Apenas propostas 'Em Análise' podem ser rejeitadas.";
}