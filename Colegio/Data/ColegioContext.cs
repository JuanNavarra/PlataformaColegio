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
        public DbSet<Col_RolModulos> Col_RolModulos { get; set; }
        public DbSet<Col_Personas> Col_Personas { get; set; }
        public DbSet<Col_Experiencia> Col_Experiencia { get; set; }
        public DbSet<Col_InsumoLaboral> Col_InsumoLaboral { get; set; }
        public DbSet<Col_Laborales> Col_Laborales { get; set; }
        public DbSet<Col_Afiliacion> Col_Afiliacion { get; set; }
        public DbSet<Col_InfoAcademica> Col_InfoAcademica { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new Col_UsuariosConfiguration());
            modelBuilder.ApplyConfiguration(new Col_RolesConfiguration());
            modelBuilder.ApplyConfiguration(new Col_ModulosConfiguration());
            modelBuilder.ApplyConfiguration(new Col_SubModulosConfiguration());
            modelBuilder.ApplyConfiguration(new Col_SubModuloModuloConfiguration());
            modelBuilder.ApplyConfiguration(new Col_RolModulosConfiguration());
            modelBuilder.ApplyConfiguration(new Col_PersonasConfiguration());
            modelBuilder.ApplyConfiguration(new Col_ExperienciaConfiguration());
            modelBuilder.ApplyConfiguration(new Col_InsumoLaboralConfiguration());
            modelBuilder.ApplyConfiguration(new Col_LaboralesConfiguration());
            modelBuilder.ApplyConfiguration(new Col_AfiliacionConfiguration());
            modelBuilder.ApplyConfiguration(new Col_InfoAcademicaConfiguration());
        }
    }
}
