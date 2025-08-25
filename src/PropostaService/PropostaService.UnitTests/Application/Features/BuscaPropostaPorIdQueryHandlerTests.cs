using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Features.BuscaPorId;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;
using System.Net;
using PropostaService.UnitTests;
using Xunit;

public class BuscaPropostaPorIdQueryHandlerTests
{
    private readonly Mock<IPropostaRepository> _mockPropostaRepository;
    private readonly Mock<ILogger<BuscaPropostaPorIdQueryHandler>> _mockLogger;
    private readonly BuscaPropostaPorIdQueryHandler _handler;
    private readonly string _cpfValido = "09966924019";

    public BuscaPropostaPorIdQueryHandlerTests()
    {
        _mockPropostaRepository = new Mock<IPropostaRepository>();
        _mockLogger = new Mock<ILogger<BuscaPropostaPorIdQueryHandler>>();
        _handler = new BuscaPropostaPorIdQueryHandler(_mockPropostaRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_QuandoPropostaNaoEncontrada_DeveRetornarNotFound()
    {
        var query = new BuscaPropostaPorIdQuery(Guid.NewGuid());
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(query.id)).ReturnsAsync((Proposta)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Mensagem.Should().Be(MensagensErroApplication.Validation.PropostaNaoEncontrada);
    }

    [Fact]
    public async Task Handle_QuandoPropostaEncontrada_DeveRetornarOkComPropostaMapeada()
    {
        var query = new BuscaPropostaPorIdQuery(Guid.NewGuid());
        var proposta = Proposta.Criar("Cliente Encontrado", _cpfValido, 3000m).Data;
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(query.id)).ReturnsAsync(proposta);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Sucesso.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(proposta.Id);
        result.Data.NomeCliente.Should().Be("Cliente Encontrado");
    }

    [Fact]
    public async Task Handle_QuandoRepositorioLancaExcecao_DeveLogarErroERetornarInternalServerError()
    {
        var query = new BuscaPropostaPorIdQuery(Guid.NewGuid());
        var exception = new Exception("Falha na conexão");
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(query.id)).ThrowsAsync(exception);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Mensagem.Should().Be(MensagensErroApplication.Exception.ErroInterno);
        _mockLogger.VerifyLogError(exception, MensagensErroApplication.Exception.ErroBuscarPropostas, Times.Once());
    }
}