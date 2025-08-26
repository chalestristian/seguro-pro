using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;

namespace PropostaService.Infrastructure.Messaging;

public class SnsEventPublisher
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly IConfiguration _configuration;

    public SnsEventPublisher(IAmazonSimpleNotificationService snsClient, IConfiguration configuration)
    {
        _snsClient = snsClient;
        _configuration = configuration;
    }

    public async Task PublishAsync<T>(T message)
    {
            var topicArn = Environment.GetEnvironmentVariable("SNS_PROPOSTA_EVENTS")!;
            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    { "MessageType", new MessageAttributeValue { DataType = "String", StringValue = typeof(T).Name } }
                }
            };
            await _snsClient.PublishAsync(request);
        }
    }