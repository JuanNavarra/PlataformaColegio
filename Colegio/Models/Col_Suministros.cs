using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Colegio.Models
{
    public class Col_Suministros
    {
        public int SuministroId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public string TipoSuministro { get; set; }
        public string Talla { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        [NotMapped]
        public int Prestado { get; set; }
    }
}
