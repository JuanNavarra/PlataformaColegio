using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_RolesConfiguration : IEntityTypeConfiguration<Col_Roles>
    {
        public void Configure(EntityTypeBuilder<Col_Roles> builder)
        {
            builder.HasKey(e => e.RolId);
            builder.Property(p => p.NombreRol).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Estado).HasMaxLength(1).HasDefaultValue("A");

            builder.ToTable("Col_Roles");
        }
    }
}
