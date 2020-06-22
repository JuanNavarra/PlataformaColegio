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
        public Task<ApiCallResult> GuardarAutorizaciones(Col_Modulos modulo, List<Col_SubModulos> subModulo, string rol, bool swRol);
        public Task<List<Col_Roles>> CargarRol();
    }
}
