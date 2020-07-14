using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_Roles
    {
        public int RolId { get; set; }
        public string NombreRol { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string Estado { get; set; }
        public string Descripcion { get; set; }
        [NotMapped]
        public DateTime? UltimoLogin { get; set; }
        public bool? Restringir { get; set; }
    }
}
