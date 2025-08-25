using System.Net;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Features.RejeitarProposta;

public class RejeitarPropostaCommandHandler : IRequestHandler<RejeitarPropostaCommand, ApplicationResult<PropostaResponse>>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IValidator<RejeitarPropostaCommand> _validator;
    private readonly ILogger<RejeitarPropostaCommandHandler> _logger;

    public RejeitarPropostaCommandHandler(IPropostaRepository propostaRepository, IValidator<RejeitarPropostaCommand> validator, ILogger<RejeitarPropostaCommandHandler> logger)
    {
        _propostaRepository = propostaRepository;
        _validator = validator;
        _logger = logger;
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
            
            var proposta = await _propostaRepository.BuscarPorIdAsync(command.id);
            
            if (proposta is null)
                return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.Validation.PropostaNaoEncontrada, (int)HttpStatusCode.NotFound);

            var propostaRejeitada = proposta.Rejeitar();
            
            if(!propostaRejeitada.Sucesso)
                return ApplicationResult<PropostaResponse>.CriarResponseErro(propostaRejeitada.MensagemErro, (int)HttpStatusCode.BadRequest);
            
            await _propostaRepository.AtualizarAsync(proposta);
            
            return ApplicationResult<PropostaResponse>.CriarResponseSucesso(new PropostaResponse(proposta.Id, proposta.NomeCliente, proposta.ValorSeguro, proposta.Status, proposta.Status.ToString(), proposta.DataCriacao), (int)HttpStatusCode.OK);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, MensagensErroApplication.Exception.ErroRejeitarProposta);
            return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.Exception.ErroInterno, (int)HttpStatusCode.InternalServerError);
        }
    }
}