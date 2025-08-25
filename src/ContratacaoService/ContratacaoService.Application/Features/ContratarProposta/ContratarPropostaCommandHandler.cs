using System.Net;
using ContratacaoService.Application.Common.Constants;
using ContratacaoService.Application.Common.Wrappers;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Interfaces;
using ContratacaoService.Domain.Common.Enum;
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
    private readonly IPropostaServiceGateway _propostaGateway;
    private readonly ILogger<ContratarPropostaCommandHandler> _logger;

    public ContratarPropostaCommandHandler(IContratacaoRepository contratacaoRepository, IValidator<ContratarPropostaCommand> validator, IPropostaServiceGateway propostaGateway, ILogger<ContratarPropostaCommandHandler> logger)
    {
        _contratacaoRepository = contratacaoRepository;
        _propostaGateway = propostaGateway;
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

            var propostaStatusDto = await _propostaGateway.GetPropostaStatusAsync(command.PropostaId);

            if (propostaStatusDto is null)
                return ApplicationResult<ContratacaoResponse>.CriarResponseErro(MensagensErroApplication.PropostaNaoEncontrada, (int)HttpStatusCode.NotFound);
           
            if (propostaStatusDto.Status != (int)PropostaStatus.Aprovada)
                return ApplicationResult<ContratacaoResponse>.CriarResponseErro(MensagensErroApplication.PropostaNaoAprovada, (int)HttpStatusCode.BadRequest);
            
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