using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropostaService.Domain.Entities;

namespace PropostaService.Infrastructure.Configurations;

public class PropostaConfiguration : IEntityTypeConfiguration<Proposta> 
{ 
    public void Configure(EntityTypeBuilder<Proposta> builder)
    {
        builder.ToTable("propostas", "public");

        builder.HasKey(p => p.Id);
    
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
    
        builder.Property(p => p.NomeCliente)
            .HasColumnName("nome_cliente")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.CpfCliente)
            .HasColumnName("cpf_cliente")
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(p => p.ValorSeguro)
            .HasColumnName("valor_seguro")
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(p => p.DataCriacao)
            .HasColumnName("data_criacao")
            .IsRequired();

        builder.Property(p => p.DataAtualizacao)
            .HasColumnName("data_atualizacao")
            .IsRequired();
    }
}