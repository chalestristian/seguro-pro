using Amazon.SimpleNotificationService;
using Amazon.SQS;
using ContratacaoService.Application.Features.ContratarProposta;
using ContratacaoService.Domain.Interfaces;
using ContratacaoService.Infrastructure;
using ContratacaoService.Infrastructure.Consumidor;
using ContratacaoService.Infrastructure.Data.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContratacaoService.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONTRATACAO_CONNECTION")!;
        services.AddDbContext<ContratacaoDbContext>(options => options.UseNpgsql(connectionString));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ContratarPropostaCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(ContratarPropostaCommand).Assembly);
        services.AddScoped<IContratacaoRepository, ContratacaoRepository>();
        services.AddHttpClient("PropostaService", client =>
        {
            client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("PROPOSTA_SERVICE_URL")!);
        });
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonSQS>();
        services.AddHostedService<PropostaAprovadaConsumer>();
        return services;
    }
}