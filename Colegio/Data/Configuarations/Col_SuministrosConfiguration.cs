using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colegio.Data.Configuarations
{
    public class Col_SuministrosConfiguration : IEntityTypeConfiguration<Col_Suministros>
    {
        public void Configure(EntityTypeBuilder<Col_Suministros> builder)
        {
            builder.HasKey(e => e.SuministroId);
            builder.Property(p => p.Nombre).HasMaxLength(250).IsRequired();
            builder.Property(p => p.Descripcion).HasMaxLength(500).IsRequired();
            builder.Property(p => p.Stock).IsRequired();
            builder.Property(p => p.FechaCreacion).IsRequired();
            builder.Property(p => p.FechaActualizacion).IsRequired(false);
            builder.Property(p => p.Talla).HasMaxLength(2).IsRequired(false);
            builder.Property(p => p.Linea).HasMaxLength(100).IsRequired();

            builder.ToTable("Col_Suministros");
        }
    }
}
