using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_Modulos
    {
        public int ModuloId { get; set; }
        public string Nombre { get; set; }
        public string Descripccion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool EsPadre { get; set; }
        public string SubModulo { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string Estado { get; set; }
        public int RolId { get; set; }
    }
}
