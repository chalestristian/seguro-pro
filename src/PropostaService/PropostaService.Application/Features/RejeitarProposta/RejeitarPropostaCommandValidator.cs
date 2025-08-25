using FluentValidation;
using PropostaService.Application.Common.Constants;

namespace PropostaService.Application.Features.RejeitarProposta;

public class RejeitarPropostaCommandValidator: AbstractValidator<RejeitarPropostaCommand>
{ 
    public RejeitarPropostaCommandValidator()
    {
        RuleFor(command => command.id)
            .NotEmpty().WithMessage(MensagensErroApplication.Validation.PropostaIdObrigatorio);
    }
}