using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colegio.Data.Configuarations
{
    public class Col_MateriasConfiguration : IEntityTypeConfiguration<Col_Materias>
    {
        public void Configure(EntityTypeBuilder<Col_Materias> builder)
        {
            builder.HasKey(e => e.MateriaId);
            builder.Property(p => p.Codigo).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Color).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
            builder.Property(p => p.Descripcion).HasMaxLength(150).IsRequired();
            builder.Property(p => p.FechaCreacion).IsRequired();
            builder.Property(p => p.FechaActualizacion).IsRequired(false);

            builder.ToTable("Col_Materias");
        }
    }
}
