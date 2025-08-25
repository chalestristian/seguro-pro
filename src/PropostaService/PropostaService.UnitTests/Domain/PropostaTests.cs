using FluentAssertions;
using PropostaService.Domain.Common.Constants;
using PropostaService.Domain.Common.Enum;
using PropostaService.Domain.Entities;
using Xunit;

namespace PropostaService.UnitTests.Domain;

public class PropostaTests
{
    private const string NomeClienteValido = "Thales Cristian";
    private const string CpfValido = "09966924019";
    private const decimal ValorSeguroValido = 5000.75m;
    
    [Fact]
    public void Criar_ComDadosValidos_DeveRetornarSucessoECriarPropostaEmAnalise()
    {
        var result = Proposta.Criar(NomeClienteValido, CpfValido, ValorSeguroValido);

        result.Sucesso.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.NomeCliente.Should().Be(NomeClienteValido);
        result.Data.CpfCliente.Should().Be(CpfValido);
        result.Data.ValorSeguro.Should().Be(ValorSeguroValido);
        result.Data.Status.Should().Be(PropostaStatus.EmAnalise);
        result.Data.Id.Should().NotBe(Guid.Empty);
    }
    
    [Xunit.Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Criar_ComNomeClienteVazioOuNulo_DeveRetornarErro(string nomeInvalido)
    {
        var result = Proposta.Criar(nomeInvalido, CpfValido, ValorSeguroValido);
        
        result.Sucesso.Should().BeFalse();
        result.MensagemErro.Should().Be(MensagensErroDomain.NomeClienteVazio);
    }

    [Fact]
    public void Criar_ComCpfInvalido_DeveRetornarErro()
    {
        var result = Proposta.Criar(NomeClienteValido, "123", ValorSeguroValido);
        
        result.Sucesso.Should().BeFalse();
        result.MensagemErro.Should().Be(MensagensErroDomain.CpfInvalido);
    }

    [Xunit.Theory]
    [InlineData(0)]
    [InlineData(-150.50)]
    public void Criar_ComValorSeguroInvalido_DeveRetornarErro(decimal valorInvalido)
    {
        var result = Proposta.Criar(NomeClienteValido, CpfValido, valorInvalido);
        
        result.Sucesso.Should().BeFalse();
        result.MensagemErro.Should().Be(MensagensErroDomain.ValorSeguroInvalido);
    }
    
    [Fact]
    public void Aprovar_PropostaEmAnalise_DeveMudarStatusParaAprovadaERetornarSucesso()
    {
        var proposta = Proposta.Criar(NomeClienteValido, CpfValido, ValorSeguroValido).Data;
        var dataAtualizacaoAntiga = proposta.DataAtualizacao;

        var result = proposta.Aprovar();

        result.Sucesso.Should().BeTrue();
        result.Data.Status.Should().Be(PropostaStatus.Aprovada);
        result.Data.DataAtualizacao.Should().BeAfter(dataAtualizacaoAntiga);
    }

    [Fact]
    public void Aprovar_PropostaJaAprovada_DeveRetornarErro()
    {
        var proposta = Proposta.Criar(NomeClienteValido, CpfValido, ValorSeguroValido).Data;
        proposta.Aprovar();

        var result = proposta.Aprovar();

        result.Sucesso.Should().BeFalse();
        result.MensagemErro.Should().Be(MensagensErroDomain.StatusNaoPermitidoParaAprovacao);
    }
    
    [Fact]
    public void Rejeitar_PropostaEmAnalise_DeveMudarStatusParaRejeitadaERetornarSucesso()
    {
        var proposta = Proposta.Criar(NomeClienteValido, CpfValido, ValorSeguroValido).Data;
        var dataAtualizacaoAntiga = proposta.DataAtualizacao;

        var result = proposta.Rejeitar();

        result.Sucesso.Should().BeTrue();
        result.Data.Status.Should().Be(PropostaStatus.Rejeitada);
        result.Data.DataAtualizacao.Should().BeAfter(dataAtualizacaoAntiga);
    }

    [Fact]
    public void Rejeitar_PropostaJaAprovada_DeveRetornarErro()
    {
        var proposta = Proposta.Criar(NomeClienteValido, CpfValido, ValorSeguroValido).Data;
        proposta.Aprovar(); 

        var result = proposta.Rejeitar();

        result.Sucesso.Should().BeFalse();
        result.MensagemErro.Should().Be(MensagensErroDomain.StatusNaoPermitidoParaRejeicao);
    }
}