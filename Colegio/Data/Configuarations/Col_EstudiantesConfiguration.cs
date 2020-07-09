using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_EstudiantesConfiguration : IEntityTypeConfiguration<Col_Estudiantes>
    {
        public void Configure(EntityTypeBuilder<Col_Estudiantes> builder)
        {
            builder.HasKey(e => e.EstudianteId);
            builder.Property(p => p.Nombres).HasMaxLength(50).IsRequired();
            builder.Property(p => p.PrimerApellido).HasMaxLength(50).IsRequired();
            builder.Property(p => p.SegundoApellido).HasMaxLength(50).IsRequired();
            builder.Property(p => p.NombresMadre).HasMaxLength(500).IsRequired();
            builder.Property(p => p.NombresPadre).HasMaxLength(500).IsRequired();
            builder.Property(p => p.NombresAcudiente).HasMaxLength(500).IsRequired();
            builder.Property(p => p.FechaNacimiento).IsRequired();
            builder.Property(p => p.NombresAcudiente).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Estado).HasMaxLength(1).HasDefaultValue("A");
            builder.Property(p => p.TelefonoAcudiente).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Direccion).HasMaxLength(100).IsRequired();
            builder.Property(p => p.NumeroDocumento).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Email).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.FechaCreacion).HasMaxLength(50).IsRequired();
            builder.Property(p => p.FechaActualizacion).HasMaxLength(50).IsRequired(false);
            builder.HasOne<Col_Usuarios>()
                .WithMany()
                .HasForeignKey(f => f.UsuarioId)
                .IsRequired();
            builder.ToTable("Col_Estudiantes");
        }
    }
}
