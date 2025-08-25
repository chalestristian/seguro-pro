using System.Net;
using FluentValidation;
using MediatR;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Features.Propostas.CriarProposta;

public class CriarPropostaCommandHandler : IRequestHandler<CriarPropostaCommand, ApplicationResult<PropostaResponse>>
{
    
    private readonly IPropostaRepository _propostaRepository;
    private readonly IValidator<CriarPropostaCommand> _validator;

    
    public CriarPropostaCommandHandler(IPropostaRepository propostaRepository, IValidator<CriarPropostaCommand> validator)
    {
        _propostaRepository = propostaRepository;
        _validator = validator;
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
            
            var proposta = Proposta.Criar(command.NomeCliente, command.CpfCliente, command.ValorSeguro);

            if (!proposta.Sucesso)
                return ApplicationResult<PropostaResponse>.CriarResponseErro(proposta.MensagemErro, (int)HttpStatusCode.BadRequest);
            
            await _propostaRepository.CriarAsync(proposta.Data);
            
            return ApplicationResult<PropostaResponse>.CriarResponseSucesso(new PropostaResponse(proposta.Data.Id, proposta.Data.ValorSeguro, proposta.Data.Status, proposta.Data.Status.ToString(), proposta.Data.DataCriacao), (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            //IMPLEMENTAR LOG
            return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.ErroInterno, (int)HttpStatusCode.InternalServerError);
        }
    }
}
