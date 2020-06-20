using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_UsuariosConfiguration : IEntityTypeConfiguration<Col_Usuarios>
    {
        public void Configure(EntityTypeBuilder<Col_Usuarios> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(p => p.Usuario).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Contrasena).HasMaxLength(20).IsRequired();
            builder.Property(p => p.Estado).HasMaxLength(1).HasDefaultValue("A");
            builder.Property(p => p.UltimaContrasena).HasMaxLength(20).IsRequired();
            builder.HasOne<Col_Roles>()
                .WithMany()
                .HasForeignKey(f => f.RolId)
                .IsRequired();

            builder.ToTable("Col_Usuarios");
        }
    }
}
