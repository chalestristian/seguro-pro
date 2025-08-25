using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Features.ListarPropostas;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;
using System.Net;
using Xunit;

namespace PropostaService.UnitTests.Application.Features;

public class ListarPropostasQueryHandlerTests
{
    private readonly Mock<IPropostaRepository> _mockPropostaRepository;
    private readonly Mock<ILogger<ListarPropostasQueryHandler>> _mockLogger;
    private readonly ListarPropostasQueryHandler _handler;
    private readonly string _cpfValido = "09966924019";

    public ListarPropostasQueryHandlerTests()
    {
        _mockPropostaRepository = new Mock<IPropostaRepository>();
        _mockLogger = new Mock<ILogger<ListarPropostasQueryHandler>>();
        _handler = new ListarPropostasQueryHandler(_mockPropostaRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_QuandoNaoExistemPropostas_DeveRetornarOkComListaVazia()
    {
        var query = new ListarPropostasQuery();
        _mockPropostaRepository.Setup(r => r.BuscarAsync()).ReturnsAsync(new List<Proposta>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Sucesso.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_QuandoExistemPropostas_DeveRetornarOkComListaMapeada()
    {
        var query = new ListarPropostasQuery();
        var propostasDoDominio = new List<Proposta>
        {
            Proposta.Criar("Cliente A", _cpfValido, 1000m).Data,
            Proposta.Criar("Cliente B", _cpfValido, 2500m).Data
        };
        _mockPropostaRepository.Setup(r => r.BuscarAsync()).ReturnsAsync(propostasDoDominio);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Sucesso.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Data.Should().HaveCount(2);
        result.Data.First().NomeCliente.Should().Be("Cliente A");
        result.Data.Last().NomeCliente.Should().Be("Cliente B");
        result.Data.First().NomeStatusProposta.Should().Be(propostasDoDominio.First().Status.ToString());
    }

    [Fact]
    public async Task Handle_QuandoRepositorioLancaExcecao_DeveLogarErroERetornarInternalServerError()
    {
        var query = new ListarPropostasQuery();
        var exception = new Exception("Erro de banco de dados");
        _mockPropostaRepository.Setup(r => r.BuscarAsync()).ThrowsAsync(exception);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Mensagem.Should().Be(MensagensErroApplication.Exception.ErroInterno);
        _mockLogger.VerifyLogError(exception, MensagensErroApplication.Exception.ErroListarPropostas, Times.Once());
    }
}