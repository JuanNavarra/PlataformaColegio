using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_Afiliacion
    {
        public int AfiliacionId { get; set; }
        public string TipoEntidad { get; set; }
        public string NombreEntidad { get; set; }
        public DateTime? FechaAfiliacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int LaboralId { get; set; }
    }
}
