using System.Reflection;
using ContratacaoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure;

public class ContratacaoDbContext : DbContext
{
    public ContratacaoDbContext(DbContextOptions<ContratacaoDbContext> options) : base(options) { }
    public DbSet<Contratacao> Contratacoes { get; set; }
    public DbSet<PropostaElegivel> PropostaElegivel { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    
}