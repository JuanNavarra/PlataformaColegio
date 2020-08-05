using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_HorariosConfiguration : IEntityTypeConfiguration<Col_Horarios>
    {
        public void Configure(EntityTypeBuilder<Col_Horarios> builder)
        {
            builder.HasKey(e => e.HorarioId);
            builder.Property(p => p.HoraIni).IsRequired().HasMaxLength(50);
            builder.Property(p => p.HoraFin).IsRequired().HasMaxLength(50);

            builder.HasOne<Col_Cursos>()
                .WithMany()
                .HasForeignKey(f => f.CursoId)
                .IsRequired();

            builder.HasOne<Col_Materias>()
                .WithMany()
                .HasForeignKey(f => f.MateriaId)
                .IsRequired();

            builder.HasOne<Col_Personas>()
                .WithMany()
                .HasForeignKey(f => f.PersonaId)
                .IsRequired(false);

            builder.ToTable("Col_Horarios");
        }
    }
}