using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models.ModelHelper
{
    public class ModulosSelect
    {
        public int ModuloId { get; set; }
        public int RolId { get; set; }
        public string Nombre { get; set; }
        public int SubModuloId { get; set; }
        public Col_Roles Rol { get; set; }
        public List<ModulosSelect> Modulos { get; set; }
        public List<ModulosSelect> Seleccionados { get; set; }
        public List<ModulosSelect> NoSeleccionados { get; set; }
        public List<ModulosSelect> AllSubModulos { get; set; }
        public List<ModulosSelect> AllModulos { get; set; }
        public string Descripcion { get; set; }
    }
}
