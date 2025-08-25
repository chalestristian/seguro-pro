using ContratacaoService.Application.Features.ContratarProposta;
using ContratacaoService.Infrastructure;
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
        //services.AddScoped<IPropostaRepository, PropostaRepository>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ContratarPropostaCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(ContratarPropostaCommand).Assembly);
        
        return services;
    }
}