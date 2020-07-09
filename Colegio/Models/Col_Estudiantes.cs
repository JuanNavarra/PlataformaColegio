using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_Estudiantes
    {
        public int EstudianteId { get; set; }
        public string Nombres { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public DateTime FechaNacimiento{ get; set; }
        public string NombresPadre { get; set; }
        public string NombresMadre { get; set; }
        public string NombresAcudiente { get; set; }
        public string TelefonoAcudiente { get; set; }
        public string Direccion { get; set; }
        public string Genero { get; set; }
        public string NumeroDocumento { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int UsuarioId { get; set; }
    }
}
