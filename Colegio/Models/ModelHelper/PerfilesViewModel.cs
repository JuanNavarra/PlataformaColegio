using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models.ModelHelper
{
    public class PerfilesViewModel
    {
        public Col_Roles Roles { get; set; }
        public List<ModulosToSubModulos> Modulos { get; set; }
        public List<UsuariosPerfiles> Usuarios { get; set; }
    }
}
