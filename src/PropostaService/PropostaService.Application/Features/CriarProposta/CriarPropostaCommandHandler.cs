using System.Net;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Common.Enum;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Features.CriarProposta;

public class CriarPropostaCommandHandler : IRequestHandler<CriarPropostaCommand, ApplicationResult<PropostaResponse>>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IValidator<CriarPropostaCommand> _validator;
    private readonly ILogger<CriarPropostaCommandHandler> _logger;
    public CriarPropostaCommandHandler(IPropostaRepository propostaRepository, IValidator<CriarPropostaCommand> validator, ILogger<CriarPropostaCommandHandler> logger)
    {
        _propostaRepository = propostaRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<PropostaResponse>> Handle(CriarPropostaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<PropostaResponse>.CriarResponseErro(string.Join("; ", errors), (int)HttpStatusCode.BadRequest);
            }

            var propostaExiste = await _propostaRepository.BuscarPeloCpfAsync(command.CpfCliente);

            if (propostaExiste is not null && propostaExiste.Status == PropostaStatus.EmAnalise) 
                return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.Validation.PropostaJaExisteEmAnalise, (int)HttpStatusCode.Conflict);
            
            var proposta = Proposta.Criar(command.NomeCliente, command.CpfCliente, command.ValorSeguro);

            if (!proposta.Sucesso)
                return ApplicationResult<PropostaResponse>.CriarResponseErro(proposta.MensagemErro, (int)HttpStatusCode.BadRequest);
            
            await _propostaRepository.CriarAsync(proposta.Data);
            
            return ApplicationResult<PropostaResponse>.CriarResponseSucesso(new PropostaResponse(proposta.Data.Id, proposta.Data.NomeCliente, proposta.Data.ValorSeguro, proposta.Data.Status, proposta.Data.Status.ToString(), proposta.Data.DataCriacao), (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MensagensErroApplication.Exception.ErroCriarProposta);
            return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.Exception.ErroInterno, (int)HttpStatusCode.InternalServerError);
        }
    }
}
