using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models.ModelHelper
{
    public class Devoluciones
    {
        public int IdPrestamo { get; set; }
        public int IdPersona { get; set; }
        public string Insumo { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public string Motivo { get; set; }
        public int SuministroId { get; set; }
        public string Descripcion { get; set; }
        public int Devolver { get; set; }
    }
}
