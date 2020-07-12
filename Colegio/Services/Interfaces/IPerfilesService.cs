using Colegio.Models;
using Colegio.Models.ModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Services.Interfaces
{
    public interface IPerfilesService
    {
        public Task<ApiCallResult> GuardarAutorizaciones(List<Col_RolModulos> modulo, List<Col_SubModuloModulo> subModulo, string rol, string descripcion);
        public Task<List<Col_Modulos>> CargarModulos();
        public Task<List<Col_SubModulos>> CargarSubModulos(int[] modulos);
        public Task<List<Col_Roles>> MostrarAutorizaciones();
        public Task<PerfilesViewModel> MostrarDetallePerfil(int rolId);
        public Task<ApiCallResult> EliminarPerfiles(int rolId, bool op);
    }
}
