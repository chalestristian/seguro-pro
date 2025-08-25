using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Application.Features.AprovarProposta;
using PropostaService.Application.Features.CriarProposta;
using PropostaService.Application.Features.ListarPropostas;
using PropostaService.Application.Features.RejeitarProposta;

namespace PropostaService.Api;

[ApiController]
[Route("api/propostas")]
public class PropostasController : ControllerBase
{
    private readonly IMediator _mediator; 
    
    public PropostasController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<ActionResult<ApplicationResult<PropostaResponse>>> Criar(CriarPropostaCommand request)
    {
        var result = await _mediator.Send(request);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpGet]
    public async Task<ActionResult<ApplicationResult<PropostaResponse>>> Buscar()
    {
        var result = await _mediator.Send(new ListarPropostasQuery());
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpPut("{id:guid}/aprovar")]
    public async Task<ActionResult<ApplicationResult<PropostaResponse>>> Aprovar([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new AprovarPropostaCommand(id));
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpPut("{id:guid}/rejeitar")]
    public async Task<ActionResult<ApplicationResult<PropostaResponse>>> Rejeitar([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new RejeitarPropostaCommand(id));
        return StatusCode(result.StatusCode, result);
    }
}