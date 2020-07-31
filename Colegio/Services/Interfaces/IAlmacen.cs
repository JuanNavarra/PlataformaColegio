using Colegio.Models;
using Colegio.Models.ModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Services.Interfaces
{
    public interface IAlmacen
    {
        public Task<ApiCallResult> GuardarSuministros(Col_Suministros suministros);
        public Task<List<Col_Suministros>> ListarSuministros();
        public Task<Col_Personas> BuscarEmpleado(string documento);
    }
}
