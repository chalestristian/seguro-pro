using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using ContratacaoService.Application.Common.Constants;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Features.ContratarProposta;
using ContratacaoService.Application.Interfaces;
using ContratacaoService.Domain.Common.Enum;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Interfaces;
using System.Net;
using FluentAssertions;
using Xunit;

public class ContratarPropostaCommandHandlerTests
{
    private readonly Mock<IContratacaoRepository> _mockContratacaoRepository;
    private readonly Mock<IValidator<ContratarPropostaCommand>> _mockValidator;
    private readonly Mock<IPropostaServiceGateway> _mockPropostaGateway;
    private readonly Mock<ILogger<ContratarPropostaCommandHandler>> _mockLogger;
    private readonly ContratarPropostaCommandHandler _handler;

    public ContratarPropostaCommandHandlerTests()
    {
        _mockContratacaoRepository = new Mock<IContratacaoRepository>();
        _mockValidator = new Mock<IValidator<ContratarPropostaCommand>>();
        _mockPropostaGateway = new Mock<IPropostaServiceGateway>();
        _mockLogger = new Mock<ILogger<ContratarPropostaCommandHandler>>();

        _handler = new ContratarPropostaCommandHandler(
            _mockContratacaoRepository.Object,
            _mockValidator.Object,
            _mockPropostaGateway.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_QuandoValidacaoDoComandoFalha_DeveRetornarBadRequest()
    {
        var command = new ContratarPropostaCommand(Guid.Empty);
        var validationFailures = new List<ValidationFailure> { new("PropostaId", "ID da proposta é obrigatório.") };
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(validationFailures));
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _mockPropostaGateway.Verify(g => g.GetPropostaStatusAsync(It.IsAny<Guid>()), Times.Never);
        _mockContratacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Contratacao>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoGatewayNaoEncontraProposta_DeveRetornarNotFound()
    {
        var command = new ContratarPropostaCommand(Guid.NewGuid());
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaGateway.Setup(g => g.GetPropostaStatusAsync(command.PropostaId)).ReturnsAsync((PropostaStatusDto)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Mensagem.Should().Be(MensagensErroApplication.PropostaNaoEncontrada);
        _mockContratacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Contratacao>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoPropostaNaoEstaAprovada_DeveRetornarBadRequest()
    {
        var command = new ContratarPropostaCommand(Guid.NewGuid());
        var statusDto = new PropostaStatusDto { StatusProposta = (int)PropostaStatus.EmAnalise };
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaGateway.Setup(g => g.GetPropostaStatusAsync(command.PropostaId)).ReturnsAsync(statusDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result.Mensagem.Should().Be(MensagensErroApplication.PropostaNaoAprovada);
        _mockContratacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Contratacao>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoFluxoExecutadoComSucesso_DeveRetornarCreated()
    {
        var command = new ContratarPropostaCommand(Guid.NewGuid());
        var statusDto = new PropostaStatusDto { StatusProposta = (int)PropostaStatus.Aprovada };
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaGateway.Setup(g => g.GetPropostaStatusAsync(command.PropostaId)).ReturnsAsync(statusDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        result.Data.Should().NotBeNull();
        result.Data.PropostaId.Should().Be(command.PropostaId);
        _mockContratacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Contratacao>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoGatewayLancaExcecao_DeveLogarErroERetornarInternalServerError()
    {
        var command = new ContratarPropostaCommand(Guid.NewGuid());
        var exception = new HttpRequestException("Falha de rede");
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaGateway.Setup(g => g.GetPropostaStatusAsync(command.PropostaId)).ThrowsAsync(exception);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        _mockLogger.VerifyLogError(exception, MensagensErroApplication.Exception.ErroCriarProposta, Times.Once());
    }
}