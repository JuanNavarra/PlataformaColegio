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
    }
}
