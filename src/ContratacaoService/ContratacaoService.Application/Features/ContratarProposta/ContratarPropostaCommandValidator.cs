using ContratacaoService.Application.Common.Constants;
using FluentValidation;

namespace ContratacaoService.Application.Features.ContratarProposta;

public class ContratarPropostaCommandValidator : AbstractValidator<ContratarPropostaCommand>
{
    public ContratarPropostaCommandValidator()
    {
        RuleFor(c => c.PropostaId)
            .NotEmpty().WithMessage(MensagensErroApplication.PropostaIdVazio);
    }
}