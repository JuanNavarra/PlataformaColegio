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
        public Task<List<Col_Materias>> MostrarMarterias();
        public Task<List<Col_Cursos>> MostrarCursos();
        public Task<ApiCallResult> AgregarMateriasHorario(Col_Horarios _horario);
        public Task<List<Horarios>> MostrarHorasMaterias(int cursoId);
        public Task<ApiCallResult> EliminarHoriorMateria(int horarioId);
    }
}
