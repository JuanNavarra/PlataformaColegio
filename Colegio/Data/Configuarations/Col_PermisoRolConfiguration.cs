using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_PermisoRolConfiguration : IEntityTypeConfiguration<Col_PermisoRol>
    {
        public void Configure(EntityTypeBuilder<Col_PermisoRol> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne<Col_Roles>()
                .WithMany()
                .HasForeignKey(f => f.RolId)
                .IsRequired();
            builder.HasOne<Col_PermisosCrud>()
                .WithMany()
                .HasForeignKey(f => f.PermisoId)
                .IsRequired();

            builder.ToTable("Col_PermisoRol");
        }
    }
}
