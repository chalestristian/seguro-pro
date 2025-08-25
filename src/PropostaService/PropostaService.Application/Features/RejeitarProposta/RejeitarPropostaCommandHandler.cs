using System.Net;
using FluentValidation;
using MediatR;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Features.RejeitarProposta;

public class RejeitarPropostaCommandHandler : IRequestHandler<RejeitarPropostaCommand, ApplicationResult<PropostaResponse>>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IValidator<RejeitarPropostaCommand> _validator;

    public RejeitarPropostaCommandHandler(IPropostaRepository propostaRepository, IValidator<RejeitarPropostaCommand> validator)
    {
        _propostaRepository = propostaRepository;
        _validator = validator;
    }

    public async Task<ApplicationResult<PropostaResponse>> Handle(RejeitarPropostaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<PropostaResponse>.CriarResponseErro(string.Join("; ", errors), (int)HttpStatusCode.BadRequest);
            }
            
            var proposta = await _propostaRepository.BuscaPorIdAsync(command.id);
            
            if (proposta is null)
                return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.PropostaNaoEncontrada, (int)HttpStatusCode.NotFound);

            var propostaAtualizada = proposta.Reprovar();
            
            if(!propostaAtualizada.Sucesso)
                return ApplicationResult<PropostaResponse>.CriarResponseErro(propostaAtualizada.MensagemErro, (int)HttpStatusCode.BadRequest);
            
            await _propostaRepository.AtualizarAsync(proposta);
            
            return ApplicationResult<PropostaResponse>.CriarResponseSucesso(new PropostaResponse(proposta.Id, proposta.NomeCliente, proposta.ValorSeguro, proposta.Status, proposta.Status.ToString(), proposta.DataCriacao), (int)HttpStatusCode.OK);
        }
        catch (Exception ex)
        {   
            //IMPLEMENTAR LOG
            return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.ErroInterno, (int)HttpStatusCode.InternalServerError);
        }
    }
}