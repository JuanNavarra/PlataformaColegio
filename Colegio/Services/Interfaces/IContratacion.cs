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
        public Task<ApiCallResult> GuardarAfiliacion(List<Col_Afiliacion> afiliaciones, int rol, int laboralId, Col_Personas persona);
        public Task<List<EmpleadosContratados>> MostrarEmpleados();
        public Task<Progresos> MostrarPendientes(char progresos, int idPersona);
        public Task<int> ActualizarPersonales(Col_Personas persona, List<Col_InfoAcademica> infoAcademicas, int personaActualizar);
        public Task<bool> ActualizarExperiencia(List<Col_Experiencia> experiencias, int experienciaActualzar);
        public Task<int> ActualizarLaboborales(Col_Laborales laboral, List<Col_InsumoLaboral> insumos, int laboralActualizar);
        public Task<ApiCallResult> ActualizarAfiliciacion(List<Col_Afiliacion> afiliaciones, int afiliacioneActualizar);
        public Task<ApiCallResult> EliminarEmpleado(int personaId, bool op);
        public Task<ApiCallResult> ActivarPerfil(int personaId);
    }
}
