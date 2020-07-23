using Colegio.Models;
using Colegio.Models.ModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Services.Interfaces
{
    public interface IContratacion
    {
        public Task<int> GuardarPersonales(Col_Personas persona, List<Col_InfoAcademica> infoAcademicas);
        public Task<bool> GuardarExperiencia(List<Col_Experiencia> experiencias, int personaId);
        public Task<int> GuardarLaboborales(Col_Laborales laboral, List<Col_InsumoLaboral> insumos, int personaId);
        public Task<List<Col_Roles>> MostrarRoles();
        public Task<ApiCallResult> GuardarAfiliacion(List<Col_Afiliacion> afiliaciones, int rol, int laboralId, string primerNombre, string primerApellido, string numeroDocumento);
    }
}
