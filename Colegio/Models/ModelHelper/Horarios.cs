using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models.ModelHelper
{
    public class Horarios
    {
        public string Materia { get; set; }
        public string Color { get; set; }
        public string HoraIni { get; set; }
        public string HoraFin { get; set; }
        public List<string> Intervalo { get; set; }
    }
}
