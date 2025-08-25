using FluentValidation;
using PropostaService.Application.Common.Constants;
using PropostaService.Domain.Common.Wrappers;

namespace PropostaService.Application.Features.Propostas.CriarProposta;

public class CriarPropostaCommandValidator : AbstractValidator<CriarPropostaCommand>
{ 
    public CriarPropostaCommandValidator()
    {
        RuleFor(p => p.NomeCliente)
            .NotEmpty().WithMessage(MensagensErroApplication.NomeVazio)
            .MaximumLength(200).WithMessage(MensagensErroApplication.NomeLongo);

        RuleFor(p => p.CpfCliente)
            .NotEmpty().WithMessage(MensagensErroApplication.CpfVazio)
            .Must(CpfValidator.IsValid).WithMessage(MensagensErroApplication.CpfInvalido);

        RuleFor(p => p.ValorSeguro)
            .GreaterThan(0).WithMessage(MensagensErroApplication.ValorSeguroInvalido);
    }
}