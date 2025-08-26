using System.Net;
using ContratacaoService.Application.Common.Constants;
using ContratacaoService.Application.Common.Wrappers;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Application.Features.ContratarProposta;

public class ContratarPropostaCommandHandler : IRequestHandler<ContratarPropostaCommand, ApplicationResult<ContratacaoResponse>>
{
    private readonly IContratacaoRepository _contratacaoRepository;
    private readonly IValidator<ContratarPropostaCommand> _validator;
    private readonly ILogger<ContratarPropostaCommandHandler> _logger;

    public ContratarPropostaCommandHandler(IContratacaoRepository contratacaoRepository, IValidator<ContratarPropostaCommand> validator, ILogger<ContratarPropostaCommandHandler> logger)
    {
        _contratacaoRepository = contratacaoRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<ApplicationResult<ContratacaoResponse>> Handle(ContratarPropostaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<ContratacaoResponse>.CriarResponseErro(string.Join("; ", errors), (int)HttpStatusCode.BadRequest);
            }

            var propostaJaCadastrada = await _contratacaoRepository.BuscarPorPropostaIdAsync(command.PropostaId);
            
            if(propostaJaCadastrada is not null)
                return ApplicationResult<ContratacaoResponse>.CriarResponseErro(MensagensErroApplication.PropostaJaCadastrada, (int)HttpStatusCode.Conflict);
            
            if(!await _contratacaoRepository.VerificarSePropostaEstaDisponivelAsync(command.PropostaId))
                return ApplicationResult<ContratacaoResponse>.CriarResponseErro(MensagensErroApplication.PropostaNaoDisponivel, (int)HttpStatusCode.Conflict);
            
            var contratacao = Contratacao.Contratar(command.PropostaId);
            
            if (!contratacao.Sucesso)
                return ApplicationResult<ContratacaoResponse>.CriarResponseErro(contratacao.MensagemErro, (int)HttpStatusCode.BadRequest);
            
            await _contratacaoRepository.AdicionarAsync(contratacao.Data);
            
            return ApplicationResult<ContratacaoResponse>.CriarResponseSucesso(new ContratacaoResponse(contratacao.Data.Id, contratacao.Data.PropostaId, contratacao.Data.DataContratacao), (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MensagensErroApplication.Exception.ErroCriarProposta);
            return ApplicationResult<ContratacaoResponse>.CriarResponseErro(MensagensErroApplication.Exception.ErroInterno, (int)HttpStatusCode.InternalServerError);
        }
    }
}