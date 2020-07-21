using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_Laborales
    {
        public int LaboralId { get; set; }
        public string NombreCargo { get; set; }
        public string Salario { get; set; }
        public string TipoContrato { get; set; }
        public string Horas { get; set; }
        public string JornadaLaboral { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string AuxilioTransporte { get; set; }
        public int PersonaId { get; set; }
        public string CorreoCorporativo { get; set; }
    }
}
