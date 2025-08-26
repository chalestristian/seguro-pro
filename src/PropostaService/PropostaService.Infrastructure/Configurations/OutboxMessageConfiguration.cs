using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropostaService.Domain.Entities;

namespace PropostaService.Infrastructure.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", "public");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id");

        builder.Property(o => o.Tipo)
            .HasColumnName("tipo")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.Conteudo)
            .HasColumnName("conteudo")
            .IsRequired();

        builder.Property(o => o.CriadoEm)
            .HasColumnName("criado_em")
            .IsRequired();

        builder.Property(o => o.ProcessadoEm)
            .HasColumnName("processado_em");
    }
}