using FluentValidation;
using PropostaService.Application.Common.Constants;

namespace PropostaService.Application.Features.AprovarProposta;

public class AprovarPropostaCommandValidator: AbstractValidator<AprovarPropostaCommand>
{ 
    public AprovarPropostaCommandValidator()
    {
        RuleFor(command => command.id)
            .NotEmpty().WithMessage(MensagensErroApplication.Validation.PropostaIdObrigatorio);
    }
}