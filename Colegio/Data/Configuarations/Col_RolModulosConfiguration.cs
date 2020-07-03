using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_RolModulosConfiguration : IEntityTypeConfiguration<Col_RolModulos>
    {
        public void Configure(EntityTypeBuilder<Col_RolModulos> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne<Col_Roles>()
                .WithMany()
                .HasForeignKey(f => f.RolId)
                .IsRequired();
            builder.HasOne<Col_Modulos>()
                .WithMany()
                .HasForeignKey(f => f.ModuloId)
                .IsRequired();

            builder.ToTable("Col_RolModulos");
        }
    }
}
