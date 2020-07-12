using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models.ModelHelper
{
    public class UsuariosPerfiles
    {
        public string NombreUsuario { get; set; }
        public string NombrePersona { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public string Estado { get; set; }
        public int Id { get; set; }
        public string PermisosSubModulo { get; set; }
        public string PermisosModulo { get; set; }
        public string SubModulos { get; set; }
        public string Modulos { get; set; }
        public string NombreRol { get; set; }
    }
}
