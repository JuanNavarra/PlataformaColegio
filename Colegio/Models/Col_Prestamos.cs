using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_Prestamos
    {
        public int PrestamoId { get; set; }
        public int PersonaId { get; set; }
        public int SuministroId { get; set; }
        public int Cantidad { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string Estado { get; set; }
    }
}
