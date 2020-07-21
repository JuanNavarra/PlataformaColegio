using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colegio.Data.Configuarations
{
    public class Col_InfoAcademicaConfiguration : IEntityTypeConfiguration<Col_InfoAcademica>
    {
        public void Configure(EntityTypeBuilder<Col_InfoAcademica> builder)
        {
            builder.HasKey(e => e.AcademicoId);
            builder.Property(p => p.NivelFormacion).HasMaxLength(50).IsRequired();
            builder.Property(p => p.TituloObtenido).HasMaxLength(50).IsRequired();
            builder.Property(p => p.NombreIns).HasMaxLength(50).IsRequired();
            builder.Property(p => p.FechaGradua).IsRequired();

            builder.HasOne<Col_Personas>()
                .WithMany()
                .HasForeignKey(f => f.PersonaId)
                .IsRequired();

            builder.ToTable("Col_InfoAcademica");
        }
    }
}
