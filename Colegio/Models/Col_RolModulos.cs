using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_RolModulos
    {
        public int Id { get; set; }
        public int RolId { get; set; }
        public int ModuloId { get; set; }
        public string PermisosCrud { get; set; }
    }
}
