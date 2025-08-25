using FluentValidation;
using PropostaService.Application.Common.Constants;
using PropostaService.Domain.Common.Wrappers;

namespace PropostaService.Application.Features.CriarProposta;

public class CriarPropostaCommandValidator : AbstractValidator<CriarPropostaCommand>
{ 
    public CriarPropostaCommandValidator()
    {
        RuleFor(p => p.NomeCliente)
            .NotEmpty().WithMessage(MensagensErroApplication.Validation.NomeClienteVazio)
            .MaximumLength(200).WithMessage(MensagensErroApplication.Validation.NomeClienteExcedeComprimentoMaximo);

        RuleFor(p => p.CpfCliente)
            .NotEmpty().WithMessage(MensagensErroApplication.Validation.CpfClienteVazio)
            .MaximumLength(11).WithMessage(MensagensErroApplication.Validation.CpfSemCaracteres)
            .Must(CpfValidator.IsValid).WithMessage(MensagensErroApplication.Validation.CpfClienteInvalido);

        RuleFor(p => p.ValorSeguro)
            .GreaterThan(0).WithMessage(MensagensErroApplication.Validation.ValorSeguroDeveSerPositivo);
    }
}