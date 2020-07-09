using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class Col_SubModuloModulo
    {
        public int Id { get; set; }
        public int ModuloId { get; set; }
        public int? SubModuloId { get; set; }
        public int RolId { get; set; }
    }
}
