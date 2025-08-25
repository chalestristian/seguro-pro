using FluentValidation;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Features.RejeitarProposta;

namespace PropostaService.Application.Features.ReprovarProposta;

public class RejeitarPropostaCommandValidator: AbstractValidator<RejeitarPropostaCommand>
{ 
    public RejeitarPropostaCommandValidator()
    {
        RuleFor(command => command.id)
            .NotEmpty().WithMessage(MensagensErroApplication.IdVazio);
    }
}