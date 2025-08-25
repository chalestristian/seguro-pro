using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropostaService.Application.Common.Wrappers;
using PropostaService.Application.DTOs;
using PropostaService.Application.Features.CriarProposta;
using PropostaService.Application.Features.ListarPropostas;

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
    public async Task<ActionResult<ApplicationResult<PropostaResponse>>> Create(CriarPropostaCommand request)
    {
        var result = await _mediator.Send(request);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new ListarPropostasQuery());
        return StatusCode(result.StatusCode, result);
    }
}