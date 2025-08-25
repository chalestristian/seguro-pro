using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Features.RejeitarProposta;
using System.Net;
using PropostaService.Domain.Common.Enum;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;
using Xunit;

namespace PropostaService.UnitTests.Application.Features;

public class RejeitarPropostaCommandHandlerTests
{
    private readonly Mock<IPropostaRepository> _mockPropostaRepository;
    private readonly Mock<IValidator<RejeitarPropostaCommand>> _mockValidator;
    private readonly Mock<ILogger<RejeitarPropostaCommandHandler>> _mockLogger;
    private readonly RejeitarPropostaCommandHandler _handler;
    private readonly string _cpfValido = "09966924019";

    public RejeitarPropostaCommandHandlerTests()
    {
        _mockPropostaRepository = new Mock<IPropostaRepository>();
        _mockValidator = new Mock<IValidator<RejeitarPropostaCommand>>();
        _mockLogger = new Mock<ILogger<RejeitarPropostaCommandHandler>>();
        
        _handler = new RejeitarPropostaCommandHandler(
            _mockPropostaRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_QuandoValidacaoDoComandoFalha_DeveRetornarBadRequest()
    {
        var command = new RejeitarPropostaCommand(Guid.Empty);
        var validationResult = new ValidationResult(new List<ValidationFailure> { new("id", "ID inválido") });
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _mockPropostaRepository.Verify(r => r.BuscarPorIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoPropostaNaoEncontrada_DeveRetornarNotFound()
    {
        var command = new RejeitarPropostaCommand(Guid.NewGuid());
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(command.id)).ReturnsAsync((Proposta)null);
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Mensagem.Should().Be(MensagensErroApplication.Validation.PropostaNaoEncontrada);
    }

    [Fact]
    public async Task Handle_QuandoRegraDeDominioImpedeRejeicao_DeveRetornarBadRequest()
    {
        var command = new RejeitarPropostaCommand(Guid.NewGuid());
        var propostaJaRejeitada = Proposta.Criar("Cliente Teste", _cpfValido, 1000m).Data;
        propostaJaRejeitada.Rejeitar();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(command.id)).ReturnsAsync(propostaJaRejeitada);
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _mockPropostaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Proposta>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoExecucaoEhBemSucedida_DeveRetornarOkComStatusRejeitada()
    {
        var command = new RejeitarPropostaCommand(Guid.NewGuid());
        var propostaEmAnalise = Proposta.Criar("Cliente Teste", _cpfValido, 1000m).Data;
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(command.id)).ReturnsAsync(propostaEmAnalise);

        var result = await _handler.Handle(command, CancellationToken.None);
        
        result.Sucesso.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data.StatusProposta.Should().Be(PropostaStatus.Rejeitada);
        _mockPropostaRepository.Verify(r => r.AtualizarAsync(propostaEmAnalise), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoRepositorioLancaExcecao_DeveLogarErroERetornarInternalServerError()
    {
        var command = new RejeitarPropostaCommand(Guid.NewGuid());
        var exception = new Exception("Erro de conexão com o banco");
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(command.id)).ThrowsAsync(exception);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Mensagem.Should().Be(MensagensErroApplication.Exception.ErroInterno);
        _mockLogger.VerifyLogError(exception, MensagensErroApplication.Exception.ErroRejeitarProposta, Times.Once());
    }
}