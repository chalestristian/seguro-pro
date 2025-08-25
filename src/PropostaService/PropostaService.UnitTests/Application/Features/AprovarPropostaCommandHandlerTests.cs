using PropostaService.Domain.Common.Enum;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PropostaService.Application.Common.Constants;
using PropostaService.Application.Features.AprovarProposta;
using System.Net;
using Xunit;

namespace PropostaService.UnitTests.Application.Features;

public class AprovarPropostaCommandHandlerTests
{
    private readonly Mock<IPropostaRepository> _mockPropostaRepository;
    private readonly Mock<IValidator<AprovarPropostaCommand>> _mockValidator;
    private readonly Mock<ILogger<AprovarPropostaCommandHandler>> _mockLogger;
    private readonly AprovarPropostaCommandHandler _handler;
    private readonly string _cpfValido = "09966924019";

    public AprovarPropostaCommandHandlerTests()
    {
        _mockPropostaRepository = new Mock<IPropostaRepository>();
        _mockValidator = new Mock<IValidator<AprovarPropostaCommand>>();
        _mockLogger = new Mock<ILogger<AprovarPropostaCommandHandler>>();
        
        _handler = new AprovarPropostaCommandHandler(
            _mockPropostaRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }
    
    [Fact]
    public async Task Handle_QuandoValidacaoDoComandoFalha_DeveRetornarBadRequest()
    {
        var command = new AprovarPropostaCommand(Guid.Empty);
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
        var command = new AprovarPropostaCommand(Guid.NewGuid());
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(command.id)).ReturnsAsync((Proposta)null);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Mensagem.Should().Be(MensagensErroApplication.Validation.PropostaNaoEncontrada);
    }
    
    [Fact]
    public async Task Handle_QuandoRegraDeDominioImpedeAprovacao_DeveRetornarBadRequest()
    {
        var command = new AprovarPropostaCommand(Guid.NewGuid());
        var propostaJaAprovada = Proposta.Criar("Cliente Teste", _cpfValido, 1000).Data;
        propostaJaAprovada.Aprovar();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(command.id)).ReturnsAsync(propostaJaAprovada);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _mockPropostaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Proposta>()), Times.Never);
    }


    [Fact]
    public async Task Handle_QuandoExecucaoEhBemSucedida_DeveRetornarOkComDados()
    {
        var command = new AprovarPropostaCommand(Guid.NewGuid());
        var propostaEmAnalise = Proposta.Criar("Cliente Teste", _cpfValido, 1000m).Data;

        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(command.id)).ReturnsAsync(propostaEmAnalise);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data.StatusProposta.Should().Be(PropostaStatus.Aprovada);
        _mockPropostaRepository.Verify(r => r.AtualizarAsync(propostaEmAnalise), Times.Once);
    }
    
    [Fact]
    public async Task Handle_QuandoRepositorioLancaExcecao_DeveLogarErroERetornarInternalServerError()
    {
        var command = new AprovarPropostaCommand(Guid.NewGuid());
        var exception = new Exception("Erro de conexão com o banco");
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockPropostaRepository.Setup(r => r.BuscarPorIdAsync(command.id)).ThrowsAsync(exception);
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Mensagem.Should().Be(MensagensErroApplication.Exception.ErroInterno);
        
        _mockLogger.VerifyLogError(exception, MensagensErroApplication.Exception.ErroAprovarProposta, Times.Once());
    }
}