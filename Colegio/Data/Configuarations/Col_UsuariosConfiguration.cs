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
           
            builder.ToTable("Col_Usuarios");
        }
    }
}
