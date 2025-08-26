using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PropostaService.Application.DTOs;
using PropostaService.Infrastructure;
using PropostaService.Infrastructure.Messaging;
public class OutboxMessageProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxMessageProcessor> _logger;

    public OutboxMessageProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxMessageProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<PropostaDbContext>();
                    var publisher = scope.ServiceProvider.GetRequiredService<SnsEventPublisher>();

                    var messages = await dbContext.OutboxMessages
                        .Where(m => m.ProcessadoEm == null)
                        .OrderBy(m => m.CriadoEm)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        var eventContent = JsonSerializer.Deserialize<PropostaAprovadaEvent>(message.Conteudo);
                        if(eventContent is not null)
                        {
                            await publisher.PublishAsync(eventContent);
                        }
                        
                        message.DataPocessado();
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages.");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}