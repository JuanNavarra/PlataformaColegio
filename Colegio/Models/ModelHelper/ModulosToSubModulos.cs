using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models.ModelHelper
{
    public class ModulosToSubModulos
    {
        public string NombreRol { get; set; }
        public string NombreModulo { get; set; }
        public string NombreSubModulo { get; set; }
        public int ModuloId { get; set; }
        public List<ModulosToSubModulos> Relaciones { get; set; }
        public List<string> Permisos { get; set; }
        public string PermisoModulo { get; set; }
        public string PermisoSubModulo { get; set; }
    }
}
