using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models.ModelHelper
{
    public class Progresos
    {
        public Col_Personas Persona { get; set; }
        public List<Col_InfoAcademica> InfoAcademicas { get; set; }
        public List<Col_Experiencia> Experiencias { get; set; }
        public Col_Laborales Laboral { get; set; }
        public List<Col_InsumoLaboral> InsumosLaborales { get; set; }
        public List<Col_Afiliacion> Afiliaciones { get; set; }
        public Col_Roles Rol { get; set; }
    }
}
