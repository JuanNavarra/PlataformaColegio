using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data.Configuarations
{
    public class Col_SubModulosConfiguration : IEntityTypeConfiguration<Col_SubModulos>
    {
        public void Configure(EntityTypeBuilder<Col_SubModulos> builder)
        {
            builder.HasKey(e => e.SubModuloId);
            builder.Property(p => p.ModuloId).IsRequired();
            builder.HasOne<Col_Modulos>()
                .WithMany()
                .HasForeignKey(f => f.ModuloId)
                .IsRequired();

            builder.ToTable("Col_SubModulos");
        }
    }
}
