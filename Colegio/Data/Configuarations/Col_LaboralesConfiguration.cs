using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colegio.Data.Configuarations
{
    public class Col_LaboralesConfiguration : IEntityTypeConfiguration<Col_Laborales>
    {
        public void Configure(EntityTypeBuilder<Col_Laborales> builder)
        {
            builder.HasKey(e => e.LaboralId);
            builder.Property(p => p.NombreCargo).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.Salario).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.TipoContrato).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.Horas).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.JornadaLaboral).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.FechaIngreso).IsRequired();
            builder.Property(p => p.FechaBaja).IsRequired(false);
            builder.Property(p => p.CorreoCorporativo).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.AuxilioTransporte).HasMaxLength(50).IsRequired(false);

            builder.HasOne<Col_Personas>()
                .WithMany()
                .HasForeignKey(f => f.LaboralId)
                .IsRequired();

            builder.ToTable("Col_Laborales");
        }
    }
}
