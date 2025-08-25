using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Features.ListarPropostas;

public class ListarPropostasQueryHandler : IRequestHandler<ListarPropostasQuery, ApplicationResult<IEnumerable<PropostaResponse>>>
{
    private readonly IPropostaRepository _propostaRepository; 
    private readonly ILogger<ListarPropostasQueryHandler> _logger;
    
    public ListarPropostasQueryHandler(IPropostaRepository propostaRepository, ILogger<ListarPropostasQueryHandler> logger)
    {
        _propostaRepository = propostaRepository;
        _logger = logger;
    }
    
    public async Task<ApplicationResult<IEnumerable<PropostaResponse>>> Handle(ListarPropostasQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var propostas = await _propostaRepository.BuscarAsync(); 
            return ApplicationResult<IEnumerable<PropostaResponse>>.CriarResponseSucesso(propostas.Select(p => new PropostaResponse(p.Id, p.NomeCliente, p.ValorSeguro, p.Status, p.Status.ToString(), p.DataCriacao)), (int)HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(MensagensErroApplication.Exception.ErroListarPropostas, ex);
            return ApplicationResult<IEnumerable<PropostaResponse>>.CriarResponseErro(MensagensErroApplication.Exception.ErroInterno, (int)HttpStatusCode.InternalServerError);
        }

    }
}