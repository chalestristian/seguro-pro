namespace PropostaService.Application.Common.Constants;

public abstract class MensagensErroApplication
{
    public static class Validation
    {
        public static readonly string PropostaIdObrigatorio = "O ID da proposta é obrigatório.";
        public static readonly string PropostaNaoEncontrada = "A proposta com o ID informado não foi encontrada.";
        public static readonly string PropostasNaoEncontrada = "Propostas não foi encontrada.";
        public static readonly string NomeClienteVazio = "O nome do cliente é obrigatório.";
        public static readonly string NomeClienteExcedeComprimentoMaximo = "O nome do cliente excede o comprimento máximo permitido.";
        public static readonly string CpfClienteVazio = "O CPF do cliente é obrigatório.";
        public static readonly string CpfClienteInvalido = "O formato do CPF informado é inválido.";
        public static readonly string ValorSeguroDeveSerPositivo = "O valor do seguro deve ser um número positivo.";
    }
    public static class Exception
    {
        public static readonly string ErroInterno = "Ocorreu um erro interno, tente novamente.";
        public static readonly string ErroCriarProposta = "Ocorreu um erro inesperado ao tentar criar a proposta para o cliente";
        public static readonly string ErroAprovarProposta = "Ocorreu um erro inesperado ao tentar aprovar a proposta para o cliente";
        public static readonly string ErroRejeitarProposta = "Ocorreu um erro inesperado ao tentar rejeitar a proposta para o cliente";
        public static readonly string ErroListarPropostas = "Ocorreu um erro inesperado ao tentar listar as propostas de clientes";
        public static readonly string ErroBuscarPropostas = "Ocorreu um erro inesperado ao tentar buscar a proposta do cliente";

    }
}