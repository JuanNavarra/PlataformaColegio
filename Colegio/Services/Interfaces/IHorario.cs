using Colegio.Models;
using Colegio.Models.ModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Services.Interfaces
{
    public interface IHorario
    {
        public Task<ApiCallResult> GuardarMaterias(Col_Materias materia);
        public Task<List<Col_Materias>> MostrarMarterias(int? cursoId);
        public Task<List<Col_Cursos>> MostrarCursos();
        public Task<ApiCallResult> AgregarMateriasHorario(Col_Horarios _horario);
        public Task<List<Horarios>> MostrarHorasMaterias(int cursoId);
        public Task<ApiCallResult> EliminarHoriorMateria(int horarioId);
        public Task<List<Col_Personas>> CargarProfesores();
        public Task<Horarios> MostrarHorarios(int busqueda);
        public Task<List<Col_Horarios>> MostrarDiasSemana(int materiaId, int cursoId);
        public Task<List<Horarios>> MostrarHorarios(string dia, int materiaId, int cursoId);
        public Task<ApiCallResult> AgregarEnlaceProfesorHorario(int idHorario, string documento);
        public Task<List<Horarios>> MostrarEnlaceProfesorHorario(string documento);
        public Task<ApiCallResult> EliminarEnlaces(int enlaceId);
    }
}
