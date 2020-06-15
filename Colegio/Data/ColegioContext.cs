using Colegio.Data.Configuarations;
using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Data
{
    public class ColegioContext : DbContext
    {
        public ColegioContext(DbContextOptions<ColegioContext> options) : base(options)
        {

        }
        public DbSet<Col_Usuarios> Col_Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new Col_UsuariosConfiguration());
        }
    }
}
