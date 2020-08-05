using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colegio.Data.Configuarations
{
    public class Col_CursosConfiguration : IEntityTypeConfiguration<Col_Cursos>
    {
        public void Configure(EntityTypeBuilder<Col_Cursos> builder)
        {
            builder.HasKey(e => e.CursoId);
            builder.Property(p => p.Nombre).HasMaxLength(50).IsRequired();
            builder.Property(p => p.FechaCreacion).IsRequired();
            builder.Property(p => p.FechaActualizacion).IsRequired(false);

            builder.ToTable("Col_Cursos");
        }
    }
}
