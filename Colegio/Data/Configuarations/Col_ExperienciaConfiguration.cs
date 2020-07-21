using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colegio.Data.Configuarations
{
    public class Col_ExperienciaConfiguration : IEntityTypeConfiguration<Col_Experiencia>
    {
        public void Configure(EntityTypeBuilder<Col_Experiencia> builder)
        {
            builder.HasKey(e => e.ExperienciaId);
            builder.Property(p => p.Empresa).HasMaxLength(250).IsRequired(false);
            builder.Property(p => p.Cargo).HasMaxLength(250).IsRequired(false);
            builder.Property(p => p.FechaInicio).IsRequired(false);
            builder.Property(p => p.FechaFin).IsRequired(false);
            builder.Property(p => p.Funciones).HasMaxLength(500).IsRequired(false);
            builder.Property(p => p.Logros).HasMaxLength(500).IsRequired(false);

            builder.HasOne<Col_Usuarios>()
                .WithMany()
                .HasForeignKey(f => f.PersonaId)
                .IsRequired();

            builder.ToTable("Col_Experiencia");
        }
    }
}
