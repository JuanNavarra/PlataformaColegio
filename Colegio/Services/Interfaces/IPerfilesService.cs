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
        public Task<ApiCallResult> GuardarAutorizaciones(List<Col_Modulos> modulo, List<Col_SubModulos> subModulo, string rol, List<Col_PermisosCrud> permisos, string descripcion);
        public Task<List<Col_Roles>> CargarRol();
        public Task<List<Col_Modulos>> CargarModulos();
        public Task<List<Col_SubModulos>> CargarSubModulos(int[] modulos);
        public Task<List<Col_PermisosCrud>> CargarPermisosCRUD();
        public Task<List<Col_Roles>> MostrarAutorizaciones();
    }
}
