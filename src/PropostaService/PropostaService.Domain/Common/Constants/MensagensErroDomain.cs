namespace PropostaService.Domain.Common.Constants;

public static class MensagensErroDomain
{
    public static readonly string NomeClienteVazio = "O nome do cliente é um campo obrigatório para a proposta.";
    public static readonly string CpfInvalido = "O CPF informado possui um formato inválido.";
    public static readonly string ValorSeguroInvalido = "O valor do seguro deve ser maior que zero.";
    public static readonly string StatusNaoPermitidoParaAprovacao = "Apenas propostas com o status 'Em Análise' podem ser aprovadas.";
    public static readonly string StatusNaoPermitidoParaRejeicao = "Apenas propostas com o status 'Em Análise' podem ser rejeitadas.";
}