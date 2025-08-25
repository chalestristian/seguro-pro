using ContratacaoService.Application.Features.ContratarProposta;
using ContratacaoService.Application.Interfaces;
using ContratacaoService.Domain.Interfaces;
using ContratacaoService.Infrastructure;
using ContratacaoService.Infrastructure.Data.Repositories;
using ContratacaoService.Infrastructure.Gateways;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContratacaoService.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ContratacaoDbContext>(options => options.UseNpgsql(connectionString));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ContratarPropostaCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(ContratarPropostaCommand).Assembly);
        services.AddScoped<IContratacaoRepository, ContratacaoRepository>();
        services.AddScoped<IPropostaServiceGateway, PropostaServiceGateway>();
        var a = configuration["Services:PropostaServiceUrl"];
        services.AddHttpClient("PropostaService", client =>
        {
            client.BaseAddress = new Uri(configuration["Services:PropostaServiceUrl"]);
        });
        return services;
    }
}