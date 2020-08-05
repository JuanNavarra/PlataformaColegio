using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_Horarios
    {
        public int HorarioId { get; set; }
        public int MateriaId { get; set; }
        public int CursoId { get; set; }
        public int PersonaId { get; set; }
        public string HoraIni { get; set; }
        public string HoraFin { get; set; }
    }
}
