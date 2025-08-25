using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Application.Features.ListarPropostas;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Features.BuscaPorId;

public class BuscaPropostaPorIdQueryHandler : IRequestHandler<BuscaPropostaPorIdQuery, ApplicationResult<PropostaResponse>>
{
    private readonly IPropostaRepository _propostaRepository; 
    private readonly ILogger<BuscaPropostaPorIdQueryHandler> _logger;
    
    public BuscaPropostaPorIdQueryHandler(IPropostaRepository propostaRepository, ILogger<BuscaPropostaPorIdQueryHandler> logger)
    {
        _propostaRepository = propostaRepository;
        _logger = logger;
    }
    
    public async Task<ApplicationResult<PropostaResponse>> Handle(BuscaPropostaPorIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var propostas = await _propostaRepository.BuscarPorIdAsync(query.id);
            
            if(propostas is null) 
                return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.Validation.PropostaNaoEncontrada, (int)HttpStatusCode.NotFound);

            return ApplicationResult<PropostaResponse>.CriarResponseSucesso(new PropostaResponse(propostas.Id, propostas.NomeCliente, propostas.ValorSeguro, propostas.Status, propostas.Status.ToString(), propostas.DataCriacao), (int)HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MensagensErroApplication.Exception.ErroBuscarPropostas);
            return ApplicationResult<PropostaResponse>.CriarResponseErro(MensagensErroApplication.Exception.ErroInterno, (int)HttpStatusCode.InternalServerError);
        }
    }
}