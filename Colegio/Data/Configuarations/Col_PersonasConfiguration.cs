using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_PersonasConfiguration : IEntityTypeConfiguration<Col_Personas>
    {
        public void Configure(EntityTypeBuilder<Col_Personas> builder)
        {
            builder.HasKey(e => e.PersonaId);
            builder.Property(p => p.PrimerNombre).HasMaxLength(50).IsRequired();
            builder.Property(p => p.SegundoNombre).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.PrimerApellido).HasMaxLength(50).IsRequired();
            builder.Property(p => p.SegundoApellido).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Celular).HasMaxLength(10).IsRequired();
            builder.Property(p => p.EstadoCivil).HasMaxLength(2).IsRequired();
            builder.Property(p => p.CorreoPesonal).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Direccion).HasMaxLength(250).IsRequired();
            builder.Property(p => p.FechaNacimiento).IsRequired();
            builder.Property(p => p.FechaCreacion).IsRequired();
            builder.Property(p => p.FechaActualizacion).IsRequired(false);
            builder.Property(p => p.Estado).HasMaxLength(1).HasDefaultValue("A");

            builder.HasOne<Col_Usuarios>()
                .WithMany()
                .HasForeignKey(f => f.UsuarioId)
                .IsRequired();

            builder.ToTable("Col_Personas");
        }
    }
}
