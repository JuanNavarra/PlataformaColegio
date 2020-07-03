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
        public DbSet<Col_Roles> Col_Roles { get; set; }
        public DbSet<Col_Modulos> Col_Modulos { get; set; }
        public DbSet<Col_SubModulos> Col_SubModulos { get; set; }
        public DbSet<Col_SubModuloModulo> Col_SubModuloModulo { get; set; }
        public DbSet<Col_PermisosCrud> Col_PermisosCrud { get; set; }
        public DbSet<Col_PermisoRol> Col_PermisoRol { get; set; }
        public DbSet<Col_RolModulos> Col_RolModulos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new Col_UsuariosConfiguration());
            modelBuilder.ApplyConfiguration(new Col_RolesConfiguration());
            modelBuilder.ApplyConfiguration(new Col_ModulosConfiguration());
            modelBuilder.ApplyConfiguration(new Col_SubModulosConfiguration());
            modelBuilder.ApplyConfiguration(new Col_SubModuloModuloConfiguration());
            modelBuilder.ApplyConfiguration(new Col_PermisosCrudConfiguration());
            modelBuilder.ApplyConfiguration(new Col_PermisoRolConfiguration());
            modelBuilder.ApplyConfiguration(new Col_RolModulosConfiguration());
        }
    }
}
