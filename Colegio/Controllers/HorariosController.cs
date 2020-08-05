using Colegio.Models;
using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Controllers
{
    public class HorariosController : Controller
    {
        private readonly IHorario service;

        public HorariosController(IHorario service)
        {
            this.service = service;
        }

        private PermisosCRUD Permisos(string modulo)
        {
            PermisosCRUD permiso = new PermisosCRUD();
            var permisos = User.Claims
                        .Where(w => w.Type.Equals(modulo) && w.Value.Contains("Maestro Administrativo")).ToList();
            permiso.PSMAPB = permisos.Where(w => w.Value.Contains("Contratacion")).Any();
            permiso.PMMAPB = permisos.Any();
            permiso.PMMAPL = permisos;
            return permiso;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuardarMarterias(string materia)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    dynamic materiaJson = JsonConvert.DeserializeObject(materia);
                    Col_Materias _materia = new Col_Materias();
                    _materia.Codigo = materiaJson.Codigo;
                    _materia.Nombre = materiaJson.Nombre;
                    _materia.Color = materiaJson.Color;
                    _materia.Descripcion = materiaJson.Descripcion;
                    var result = await service.GuardarMaterias(_materia);
                    return Json(result);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MostrarMarterias()
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var leer = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Leer")).Any();
                if (leer)
                {
                    var result = await service.MostrarMarterias();
                    return Json(result);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMateriasHorario(string horario)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    dynamic horarioJson = JsonConvert.DeserializeObject(horario);
                    Col_Horarios _horario = new Col_Horarios();
                    _horario.MateriaId = horarioJson.MateriaId;
                    _horario.CursoId = horarioJson.CursoId;
                    _horario.HoraIni = horarioJson.HoraIni;
                    _horario.HoraFin = horarioJson.HoraFin;
                    var result = await service.AgregarMateriasHorario(_horario);
                    return Json(result);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpGet]
        public async Task<IActionResult> MostrarCursos()
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var leer = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Leer")).Any();
                if (leer)
                {
                    var result = await service.MostrarCursos();
                    return Json(result);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MostrarHorasMaterias(int cursoId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var leer = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Leer")).Any();
                if (leer)
                {
                    var result = await service.MostrarHorasMaterias(cursoId);
                    return Json(result);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}
