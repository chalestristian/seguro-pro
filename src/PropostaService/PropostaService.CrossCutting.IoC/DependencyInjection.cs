using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Application.Features.CriarProposta;
using PropostaService.Domain.Interfaces;
using PropostaService.Infrastructure;
using PropostaService.Infrastructure.Repository;

namespace PropostaService.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<PropostaDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPropostaRepository, PropostaRepository>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CriarPropostaCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(CriarPropostaCommand).Assembly);
        
        return services;
    }
}