using System.Net;
using MediatR;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Features.ListarPropostas;

public class ListarPropostasQueryHandler : IRequestHandler<ListarPropostasQuery, ApplicationResult<IEnumerable<PropostaResponse>>> {
    
    private readonly IPropostaRepository _propostaRepository; 
    
    public ListarPropostasQueryHandler(IPropostaRepository propostaRepository)
    {
        _propostaRepository = propostaRepository;
    }
    
    public async Task<ApplicationResult<IEnumerable<PropostaResponse>>> Handle(ListarPropostasQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var propostas = await _propostaRepository.BuscaAsync(); 
            
            return ApplicationResult<IEnumerable<PropostaResponse>>.CriarResponseSucesso(propostas.Select(p => new PropostaResponse(p.Id, p.NomeCliente, p.ValorSeguro, p.Status, p.Status.ToString(), p.DataCriacao)), (int)HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            //ADD LOG
            return ApplicationResult<IEnumerable<PropostaResponse>>.CriarResponseErro(MensagensErroApplication.ErroInterno, (int)HttpStatusCode.InternalServerError);
        }

    }
}