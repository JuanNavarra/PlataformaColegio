using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colegio.Models;
using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Colegio.Controllers
{
    public class ContratacionController : Controller
    {
        private readonly IContratacion service;
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
            if (User.Identity.IsAuthenticated)
            {
                if (Permisos("PermisoSubModulo").PSMAPB || (!Permisos("PermisoSubModulo").PSMAPB && Permisos("PermisoModulo").PMMAPB))
                {
                    string modulo = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                    var permisos = Permisos(modulo).PMMAPL;
                    ViewBag.Leer = permisos.Where(w => w.Value.Contains("Leer")).Any();
                    ViewBag.Crear = permisos.Where(w => w.Value.Contains("Crear")).Any();
                    ViewBag.Actualizar = permisos.Where(w => w.Value.Contains("Actualizar")).Any();
                    ViewBag.Eliminar = permisos.Where(w => w.Value.Contains("Eliminar")).Any();

                    return View();
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPersonales(string personal, string academico)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                var actualizar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Actualizar")).Any();
                if (crear)
                {
                    dynamic personalJson = JsonConvert.DeserializeObject(personal);
                    dynamic academicoJson = JsonConvert.DeserializeObject(academico);

                    Col_Personas personas = new Col_Personas();
                    personas.NumeroDocumento = personalJson[0].NumeroDocumento;
                    personas.Barrio = personalJson[0].Barrio;
                    personas.Celular = personalJson[0].Celular;
                    personas.CorreoPesonal = personalJson[0].CorreoPesonal;
                    personas.Direccion = personalJson[0].Direccion;
                    personas.Empleado = true;
                    personas.EstadoCivil = personalJson[0].EstadoCivil;
                    personas.FechaNacimiento = personalJson[0].FechaNacimiento;
                    personas.TipoDocumento = personalJson[0].TipoDocumento;
                    personas.PrimerNombre = personalJson[0].PrimerNombre;
                    personas.SegundoNombre = personalJson[0].SegundoNombre;
                    personas.PrimerApellido = personalJson[0].PrimerApellido;
                    personas.SegundoApellido = personalJson[0].PrimerApellido;

                    List<Col_InfoAcademica> infoAcademicas = new List<Col_InfoAcademica>();

                    foreach (var item in academicoJson)
                    {
                        Col_InfoAcademica infoAcademica = new Col_InfoAcademica();
                        infoAcademica.FechaGradua = item.FechaGradua;
                        infoAcademica.NivelFormacion = item.NivelFormacion;
                        infoAcademica.NombreIns = item.NombreIns;
                        infoAcademica.TituloObtenido = item.TituloObtenido;
                        infoAcademicas.Add(infoAcademica);
                    }

                    var result = ""// await service.GuardarPersonales(personas, infoAcademicas);
                    return Json(result);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }
    }
}
