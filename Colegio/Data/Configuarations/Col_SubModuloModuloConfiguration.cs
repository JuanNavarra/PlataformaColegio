using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_SubModuloModuloConfiguration : IEntityTypeConfiguration<Col_SubModuloModulo>
    {
        public void Configure(EntityTypeBuilder<Col_SubModuloModulo> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne<Col_Modulos>()
                .WithMany()
                .HasForeignKey(f => f.ModuloId)
                .IsRequired();
            builder.HasOne<Col_Roles>()
                .WithMany()
                .HasForeignKey(f => f.RolId)
                .IsRequired();
            builder.HasOne<Col_SubModulos>()
                .WithMany()
                .HasForeignKey(f => f.SubModuloId)
                .IsRequired(false);

            builder.ToTable("Col_SubModuloModulo");
        }
    }
}
