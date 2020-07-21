using System;

namespace Colegio.Models
{
    public class Col_InfoAcademica
    {
        public int AcademicoId { get; set; }
        public string NivelFormacion { get; set; }
        public string TituloObtenido { get; set; }
        public string NombreIns { get; set; }
        public DateTime FechaGradua { get; set; }
        public int PersonaId { get; set; }
    }
}
