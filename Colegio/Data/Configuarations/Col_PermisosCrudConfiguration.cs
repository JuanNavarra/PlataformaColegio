using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_PermisosCrudConfiguration : IEntityTypeConfiguration<Col_PermisosCrud>
    {
        public void Configure(EntityTypeBuilder<Col_PermisosCrud> builder)
        {
            builder.HasKey(e => e.PermisoId);
            builder.Property(p => p.Nombre).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Nombre).HasMaxLength(1).IsRequired().HasDefaultValue("A");
            builder.ToTable("Col_PermisosCrud");
        }
    }
}
