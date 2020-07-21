using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_Experiencia
    {
        public int ExperienciaId { get; set; }
        public string Empresa { get; set; }
        public string Cargo { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Funciones { get; set; }
        public string Logros { get; set; }
        public int PersonaId { get; set; }
    }
}
