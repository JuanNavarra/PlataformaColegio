using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Colegio.Models.ModelHelper
{
    public class PermisosCRUD
    {
        public bool PSMAPB { get; set; } //Permiso submodulo maestro administrativo perefil bool
        public bool PMMAPB { get; set; } //Permiso modulo maestro administrativo perefil bool
        public List<Claim> PMMAPL { get; set; } //Permiso modulo y sub maestro administrativo perefil lista
    }
}
