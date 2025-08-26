using Amazon.SimpleNotificationService;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Application.Features.CriarProposta;
using PropostaService.Domain.Interfaces;
using PropostaService.Infrastructure;
using PropostaService.Infrastructure.Messaging;
using PropostaService.Infrastructure.Repository;

namespace PropostaService.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("DEFAULT_PROPOSTA_CONNECTION")!;
        services.AddDbContext<PropostaDbContext>(options => options.UseNpgsql(connectionString));
        services.AddHostedService<OutboxMessageProcessor>();
        services.AddScoped<IPropostaRepository, PropostaRepository>();
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonSimpleNotificationService>();
        services.AddScoped<SnsEventPublisher>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CriarPropostaCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(CriarPropostaCommand).Assembly);
        
        return services;
    }
}