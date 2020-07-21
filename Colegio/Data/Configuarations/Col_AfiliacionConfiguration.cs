using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colegio.Data.Configuarations
{
    public class Col_AfiliacionConfiguration : IEntityTypeConfiguration<Col_Afiliacion>
    {
        public void Configure(EntityTypeBuilder<Col_Afiliacion> builder)
        {
            builder.HasKey(e => e.AfiliacionId);
            builder.Property(p => p.TipoEntidad).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.NombreEntidad).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.FechaAfiliacion).IsRequired(false);
            builder.Property(p => p.FechaActualizacion).IsRequired(false);
            builder.Property(p => p.FechaCreacion).IsRequired(false);

            builder.HasOne<Col_Laborales>()
                .WithMany()
                .HasForeignKey(f => f.LaboralId)
                .IsRequired();

            builder.ToTable("Col_Afiliacion");
        }
    }
}
