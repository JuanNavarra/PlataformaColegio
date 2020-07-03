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
            builder.Property(p => p.Nombre).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Descripcion).HasMaxLength(500).IsRequired(false);
            builder.Property(p => p.FechaActualizacion).IsRequired(false);
            builder.Property(p => p.Estado).HasMaxLength(1).HasDefaultValue("A");
            builder.ToTable("Col_Modulos");
        }
    }
}
