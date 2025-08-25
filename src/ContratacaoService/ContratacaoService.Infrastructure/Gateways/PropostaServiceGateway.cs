using System.Net;
using System.Text.Json;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Interfaces;

namespace ContratacaoService.Infrastructure.Gateways;

public class PropostaServiceGateway : IPropostaServiceGateway
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PropostaServiceGateway(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PropostaStatusDto?> GetPropostaStatusAsync(Guid propostaId)
    {
        var client = _httpClientFactory.CreateClient("PropostaService");

        var response = await client.GetAsync($"api/propostas/{propostaId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync();
        
        var resultDto = await JsonSerializer.DeserializeAsync<PropostaStatusDto>(
            responseStream, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        return resultDto;
    }
}