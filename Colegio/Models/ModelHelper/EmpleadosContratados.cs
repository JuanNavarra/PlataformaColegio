using System;

namespace Colegio.Models.ModelHelper
{
    public class EmpleadosContratados
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Perfil { get; set; }
        public DateTime Ingreso { get; set; }
        public DateTime Creacion { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public string Celular { get; set; }
        public string Correo { get; set; }
        public string Estado { get; set; }
        public char Progreso { get; set; }
        public int UsuarioId { get; set; }
    }
}
