using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;

namespace PropostaService.Infrastructure;

public class PropostaDbContext: DbContext
{
    public PropostaDbContext(DbContextOptions<PropostaDbContext> options) : base(options) { }
    public DbSet<Proposta> Propostas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}