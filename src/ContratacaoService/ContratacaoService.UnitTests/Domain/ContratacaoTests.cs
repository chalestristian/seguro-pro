using ContratacaoService.Domain.Common.Constants;
using FluentAssertions;
using ContratacaoService.Domain.Entities;
using Xunit;

public class ContratacaoTests
{
    [Fact]
    public void Contratar_QuandoPropostaIdEhVazio_DeveRetornarErro()
    {
        var propostaIdInvalida = Guid.Empty;

        var result = Contratacao.Contratar(propostaIdInvalida);

        result.Sucesso.Should().BeFalse();
        result.MensagemErro.Should().Be(MensagensErroDomain.PropostaIdVazio);
        result.Data.Should().BeNull();
    }

    [Fact]
    public void Contratar_QuandoPropostaIdEhValido_DeveRetornarSucessoECriarContratacao()
    {
        var propostaIdValida = Guid.NewGuid();

        var result = Contratacao.Contratar(propostaIdValida);

        result.Sucesso.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.PropostaId.Should().Be(propostaIdValida);
        result.Data.DataContratacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}