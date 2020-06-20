using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_ModulosConfiguration : IEntityTypeConfiguration<Col_Modulos>
    {
        public void Configure(EntityTypeBuilder<Col_Modulos> builder)
        {
            builder.HasKey(e => e.ModuloId);
            builder.HasKey(e => e.Nombre);
            builder.Property(p => p.Nombre).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Descripccion).HasMaxLength(500).IsRequired(false);
            builder.Property(p => p.SubModulo).IsRequired(false).HasMaxLength(50);
            builder.Property(p => p.FechaActualizacion).IsRequired(false);
            builder.Property(p => p.EsPadre).HasMaxLength(1).IsRequired(true);
            builder.Property(p => p.Estado).HasMaxLength(1).HasDefaultValue("A");
            builder.HasOne<Col_Roles>()
                .WithMany()
                .HasForeignKey(f => f.RolId)
                .IsRequired();

            builder.ToTable("Col_Modulos");
        }
    }
}
