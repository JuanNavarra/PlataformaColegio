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
        public int Id { get; set; }
        public int orden { get; set; }
        public string Curso { get; set; }
        public Col_Personas Profesor { get; set; }
        public List<Horarios> HDisponibles { get; set; }
        public List<Horarios> HProfesores { get; set; }
        public string Horario { get; set; }
        public List<Horarios> horarios { get; set; }
        public List<Col_Cursos> Cursos { get; set; }
        public string Dia { get; set; }
    }
}
