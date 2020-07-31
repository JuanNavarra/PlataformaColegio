using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colegio.Data.Configuarations
{
    public class Col_PrestamosConfiguration : IEntityTypeConfiguration<Col_Prestamos>
    {
        public void Configure(EntityTypeBuilder<Col_Prestamos> builder)
        {
            builder.HasKey(e => e.PrestamoId);
            builder.Property(p => p.Motivo).HasMaxLength(1000).IsRequired();
            builder.Property(p => p.Cantidad).IsRequired();
            builder.Property(p => p.FechaPrestamo).IsRequired();
            builder.Property(p => p.FechaActualizacion).IsRequired(false);
            builder.Property(p => p.FechaCreacion).IsRequired();

            builder.HasOne<Col_Personas>()
                .WithMany()
                .HasForeignKey(f => f.PersonaId)
                .IsRequired();

            builder.HasOne<Col_Suministros>()
                .WithMany()
                .HasForeignKey(f => f.SuministroId)
                .IsRequired();

            builder.ToTable("Col_Prestamos");
        }
    }
}