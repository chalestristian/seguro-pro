using FluentValidation;
using PropostaService.Application.Common.Constants;

namespace PropostaService.Application.Features.BuscaPorId;

public class BuscaPropostaPorIdQueryValidator: AbstractValidator<BuscaPropostaPorIdQuery>
{
    public BuscaPropostaPorIdQueryValidator()
    {
        RuleFor(p => p.id)
            .NotEmpty().WithMessage(MensagensErroApplication.Validation.PropostaIdObrigatorio);
    }
}