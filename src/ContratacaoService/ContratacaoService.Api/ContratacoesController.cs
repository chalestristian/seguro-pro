using ContratacaoService.Application.Common.Wrappers;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Features.ContratarProposta;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContratacaoService.Api;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContratacoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationResult<ContratacaoResponse>>> Contratar([FromBody] ContratarPropostaCommand request)
    {
        var result = await _mediator.Send(new ContratarPropostaCommand(request.PropostaId));
        return StatusCode(result.StatusCode, result);
    }
}