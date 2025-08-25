namespace ContratacaoService.Application.Common.Constants;

public abstract class MensagensErroApplication
{
    public static readonly string PropostaIdVazio = "O ID da proposta não pode ser vazio.";
    public static readonly string PropostaNaoEncontrada = "A proposta com o id informado não foi encontrada";
    public static readonly string PropostaNaoAprovada = "A proposta com o id informado não foi aprovada";


    public static class Exception
    {
        public static readonly string ErroInterno = "Ocorreu um erro interno, tente novamente.";
        public static readonly string ErroCriarProposta = "Ocorreu um erro inesperado ao tentar contratar a proposta para o cliente";
    }
}