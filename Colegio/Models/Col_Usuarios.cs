using System;

namespace Colegio.Models
{
    public class Col_Usuarios
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int RolId { get; set; }
    }
}
