using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Features.CriarProposta;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;
using System.Net;
using PropostaService.Domain.Common.Constants;
using Xunit;

namespace PropostaService.UnitTests.Application.Features;

public class CriarPropostaCommandHandlerTests
{
    private readonly Mock<IPropostaRepository> _mockPropostaRepository;
    private readonly Mock<IValidator<CriarPropostaCommand>> _mockValidator;
    private readonly Mock<ILogger<CriarPropostaCommandHandler>> _mockLogger;
    private readonly CriarPropostaCommandHandler _handler;
    private readonly string _cpfValido = "09966924019";


    public CriarPropostaCommandHandlerTests()
    {
        _mockPropostaRepository = new Mock<IPropostaRepository>();
        _mockValidator = new Mock<IValidator<CriarPropostaCommand>>();
        _mockLogger = new Mock<ILogger<CriarPropostaCommandHandler>>();

        _handler = new CriarPropostaCommandHandler(
            _mockPropostaRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_QuandoValidacaoDeEntradaFalha_DeveRetornarBadRequestSemChamarRepositorio()
    {
        var command = new CriarPropostaCommand("", _cpfValido, 1000m);
        var validationFailures = new List<ValidationFailure> { new("NomeCliente", MensagensErroApplication.Validation.NomeClienteVazio) };
        var validationResult = new ValidationResult(validationFailures);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _mockPropostaRepository.Verify(r => r.CriarAsync(It.IsAny<Proposta>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoRegraDeDominioFalha_DeveRetornarBadRequestSemChamarRepositorio()
    {
        var command = new CriarPropostaCommand("Cliente Valido", _cpfValido, -100m);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result.Mensagem.Should().Be(MensagensErroDomain.ValorSeguroInvalido);
        _mockPropostaRepository.Verify(r => r.CriarAsync(It.IsAny<Proposta>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoDadosSaoValidos_DeveChamarRepositorioERetornarCreated()
    {
        var command = new CriarPropostaCommand("Cliente Valido", _cpfValido, 5000m);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        result.Data.Should().NotBeNull();
        result.Data.NomeCliente.Should().Be(command.NomeCliente);
        result.Data.ValorSeguro.Should().Be(command.ValorSeguro);
        _mockPropostaRepository.Verify(r => r.CriarAsync(It.IsAny<Proposta>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoRepositorioLancaExcecao_DeveLogarErroERetornarInternalServerError()
    {
        var command = new CriarPropostaCommand("Cliente Valido", _cpfValido, 5000m);
        var exception = new Exception("Erro de infraestrutura");
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaRepository.Setup(r => r.CriarAsync(It.IsAny<Proposta>())).ThrowsAsync(exception);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Mensagem.Should().Be(MensagensErroApplication.Exception.ErroInterno);
        _mockLogger.VerifyLogError(exception, MensagensErroApplication.Exception.ErroCriarProposta, Times.Once());
    }
}