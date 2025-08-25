namespace PropostaService.Domain.Common.Wrappers;

public abstract class CpfValidator
{
    public static bool IsValid(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;

        cpf = new string(cpf.Where(char.IsDigit).ToArray());
        if (cpf.Length != 11) return false;

        if (cpf.Distinct().Count() == 1) return false;

        int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf[..9];
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += (tempCpf[i] - '0') * mult1[i];

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        tempCpf += digito1;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += (tempCpf[i] - '0') * mult2[i];

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return cpf.EndsWith($"{digito1}{digito2}");
    }
}