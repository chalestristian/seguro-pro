using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Infrastructure.Consumidor;
public record PropostaAprovadaEvent(Guid PropostaId);
public class PropostaAprovadaConsumer : BackgroundService
{
    private readonly ILogger<PropostaAprovadaConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _queueUrl;
    private readonly IAmazonSQS _sqsClient;

    public PropostaAprovadaConsumer(IConfiguration configuration, IAmazonSQS sqsClient, IServiceScopeFactory scopeFactory, ILogger<PropostaAprovadaConsumer> logger)
    {
        _queueUrl = Environment.GetEnvironmentVariable("SQS_PROPOSTA_EVENTS")!;
        _sqsClient = sqsClient;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                WaitTimeSeconds = 20
            };

            var response = await _sqsClient.ReceiveMessageAsync(receiveRequest, stoppingToken);
            
            if(response.Messages is not null)
            {
                foreach (var message in response.Messages)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(message.Body))
                        {
                            _logger.LogWarning("Mensagem vazia recebida: {MessageId}", message.MessageId);
                            continue;
                        }

                        string snsMessageBody;

                        using var doc = JsonDocument.Parse(message.Body);
                        if (doc.RootElement.TryGetProperty("Message", out var messageProperty))
                        {
                            snsMessageBody = messageProperty.GetString()!;
                        }
                        else
                        {
                            snsMessageBody = message.Body;
                        }

                        var proposalEvent = JsonSerializer.Deserialize<PropostaAprovadaEvent>(snsMessageBody);

                        if (proposalEvent != null)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var repo = scope.ServiceProvider.GetRequiredService<IContratacaoRepository>();
                            await repo.AdicionarPropostaElegivelAsync(new PropostaElegivel(proposalEvent.PropostaId));
                        }

                        await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ocorreu um erro ao processar a mensagem: {MessageId}", message.MessageId);
                    }
                } 
            }
        }
    }
}