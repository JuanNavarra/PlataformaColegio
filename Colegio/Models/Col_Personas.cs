using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_Personas
    {
        public int PersonaId { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Barrio { get; set; }
        public string Genero { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string Estado { get; set; }
        public int UsuarioId { get; set; }
        public string Celular { get; set; }
        public string EstadoCivil { get; set; }
        public bool Empleado { get; set; }
        public string Direccion { get; set; }
        public string CorreoPersonal { get; set; }
        public char Progreso { get; set; }
        [NotMapped]
        public string Usuario { get; set; }

    }
}
