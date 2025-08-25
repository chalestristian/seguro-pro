using System.Net;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Features.AprovarProposta;

public class AprovarPropostaCommandHandler : IRequestHandler<AprovarPropostaCommand, ApplicationResult<PropostaResponse>>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IValidator<AprovarPropostaCommand> _validator;
    private readonly ILogger<AprovarPropostaCommandHandler> _logger;

    public AprovarPropostaCommandHandler(IPropostaRepository propostaRepository, IValidator<AprovarPropostaCommand> validator, ILogger<AprovarPropostaCommandHandler> logger)
    {
        _propostaRepository = propostaRepository;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<ApplicationResult<PropostaResponse>> Handle(AprovarPropostaCommand command, CancellationToken cancellationToken)
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

            var propostaAprovada = proposta.Aprovar();
            
            if(!propostaAprovada.Sucesso)
                return ApplicationResult<PropostaResponse>.CriarResponseErro(propostaAprovada.MensagemErro, (int)HttpStatusCode.BadRequest);
            
            await _propostaRepository.AtualizarAsync(proposta);
            
            return ApplicationResult<PropostaResponse>.CriarResponseSucesso(new PropostaResponse(proposta.Id, proposta.NomeCliente, proposta.ValorSeguro, proposta.Status, proposta.Status.ToString(), proposta.DataCriacao), (int)HttpStatusCode.OK);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, MensagensErroApplication.Exception.ErroAprovarProposta);
            return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.Exception.ErroInterno, (int)HttpStatusCode.InternalServerError);
        }
    }
}