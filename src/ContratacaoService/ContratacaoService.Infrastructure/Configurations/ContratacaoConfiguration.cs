using ContratacaoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContratacaoService.Infrastructure.Configurations;

public class ContratacaoConfiguration : IEntityTypeConfiguration<Contratacao> 
{ 
    public void Configure(EntityTypeBuilder<Contratacao> builder)
    {
        builder.ToTable("contratacoes", "public");

        builder.HasKey(c => c.Id);
    
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
    
        builder.Property(c => c.PropostaId)
            .HasColumnName("proposta_id")
            .IsRequired();
        
        builder.Property(c => c.DataContratacao)
            .HasColumnName("data_contratacao")
            .IsRequired();
    }
}