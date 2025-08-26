using ContratacaoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContratacaoService.Infrastructure.Configurations;

public class PropostaElegivelConfiguration : IEntityTypeConfiguration<PropostaElegivel> 
{ 
    public void Configure(EntityTypeBuilder<PropostaElegivel> builder)
    {
        builder.ToTable("propostas_elegiveis", "public");

        builder.HasKey(c => c.PropostaId);

        builder.Property(c => c.PropostaId)
            .HasColumnName("proposta_id");
        
        builder.Property(c => c.AprovadaEm)
            .HasColumnName("aprovada_em")
            .IsRequired();
    }
}