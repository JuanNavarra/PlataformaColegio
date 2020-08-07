using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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

        public ContratacionController(IContratacion service)
        {
            this.service = service;
        }

        private PermisosCRUD Permisos(string modulo)
        {
            PermisosCRUD permiso = new PermisosCRUD();
            List<System.Security.Claims.Claim> permisos = User.Claims
                        .Where(w => w.Type.Equals(modulo) && w.Value.Contains("Maestro Administrativo")).ToList();
            permiso.PSMAPB = permisos.Where(w => w.Value.Contains("Contratacion")).Any();
            permiso.PMMAPB = permisos.Any();
            permiso.PMMAPL = permisos;
            return permiso;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Permisos("PermisoSubModulo").PSMAPB || (!Permisos("PermisoSubModulo").PSMAPB && Permisos("PermisoModulo").PMMAPB))
                {
                    List<EmpleadosContratados> registros = await service.MostrarEmpleados();
                    ViewBag.Registros = registros;

                    string modulo = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                    List<System.Security.Claims.Claim> permisos = Permisos(modulo).PMMAPL;
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
        public async Task<IActionResult> GuardarCambiosPersonales(string personal, string academico, int personaActualizar)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                bool crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                bool Actualizar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Actualizar")).Any();
                if (crear || (Actualizar && personaActualizar != 0))
                {
                    dynamic personalJson = JsonConvert.DeserializeObject(personal);
                    dynamic academicoJson = JsonConvert.DeserializeObject(academico);

                    Col_Personas persona = new Col_Personas();
                    persona.PrimerNombre = personalJson.PrimerNombre;
                    persona.Barrio = personalJson.Barrio;
                    persona.NumeroDocumento = personalJson.NumeroDocumento;
                    persona.Celular = personalJson.Celular;
                    persona.CorreoPersonal = personalJson.CorreoPersonal;
                    persona.Direccion = personalJson.Direccion;
                    persona.EstadoCivil = personalJson.EstadoCivil;
                    persona.FechaNacimiento = Convert.ToDateTime(personalJson.FechaNacimiento.ToString());
                    persona.TipoDocumento = personalJson.TipoDocumento;
                    persona.SegundoNombre = personalJson.SegundoNombre;
                    persona.PrimerApellido = personalJson.PrimerApellido;
                    persona.SegundoApellido = personalJson.SegundoApellido;

                    List<Col_InfoAcademica> infoAcademicas = new List<Col_InfoAcademica>();
                    foreach (dynamic item in academicoJson)
                    {
                        Col_InfoAcademica infoAcademica = new Col_InfoAcademica();
                        infoAcademica.FechaGradua = Convert.ToDateTime(item.FechaGradua.ToString());
                        infoAcademica.NivelFormacion = item.NivelFormacion;
                        infoAcademica.NombreIns = item.NombreIns;
                        infoAcademica.TituloObtenido = item.TituloObtenido;
                        infoAcademicas.Add(infoAcademica);
                    }

                    int result = personaActualizar == 0 ? await service.GuardarPersonales(persona, infoAcademicas)
                        : await service.ActualizarPersonales(persona, infoAcademicas, personaActualizar);
                    return Json(result);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarExperiencia(string experiencia, int personaId, int experienciaActualzar)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                bool crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                bool Actualizar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Actualizar")).Any();
                if (crear || (Actualizar && experienciaActualzar != 0))
                {
                    dynamic experienciaJson = JsonConvert.DeserializeObject(experiencia);
                    List<Col_Experiencia> experiencias = new List<Col_Experiencia>();

                    foreach (dynamic item in experienciaJson)
                    {
                        Col_Experiencia _experiencia = new Col_Experiencia();
                        _experiencia.Cargo = item.Cargo;
                        _experiencia.Empresa = item.Empresa;
                        _experiencia.FechaFin = Convert.ToDateTime(item.FechaFin.ToString());
                        _experiencia.FechaInicio = Convert.ToDateTime(item.FechaInicio.ToString());
                        _experiencia.Logros = item.Logros;
                        _experiencia.Funciones = item.Funciones;
                        experiencias.Add(_experiencia);
                    }
                    bool result = experienciaActualzar == 0 ? await service.GuardarExperiencia(experiencias, personaId)
                        : await service.ActualizarExperiencia(experiencias, experienciaActualzar);
                    return Json(result);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarLaboral(string laboral, string insumos, int personaId, int laboralActualizar)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                bool crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                bool Actualizar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Actualizar")).Any();
                if (crear || (Actualizar && laboralActualizar != 0))
                {
                    dynamic laboralJson = JsonConvert.DeserializeObject(laboral);
                    dynamic insumosJson = JsonConvert.DeserializeObject(insumos);

                    Col_Laborales _laboral = new Col_Laborales();
                    _laboral.AuxilioTransporte = laboralJson.AuxilioTransporte;
                    _laboral.CorreoCorporativo = laboralJson.CorreoCorporativo;
                    _laboral.FechaBaja = laboralJson.FechaBaja == "" ? null : Convert.ToDateTime(laboralJson.FechaBaja.ToString());
                    _laboral.FechaIngreso = Convert.ToDateTime(laboralJson.FechaIngreso.ToString());
                    _laboral.Horas = laboralJson.Horas;
                    _laboral.JornadaLaboral = laboralJson.JornadaLaboral;
                    _laboral.NombreCargo = laboralJson.NombreCargo;
                    _laboral.Salario = laboralJson.Salario;
                    _laboral.TipoContrato = laboralJson.TipoContrato;

                    List<Col_InsumoLaboral> insumoLaborales = null;
                    if (insumosJson.Count > 0)
                    {
                        insumoLaborales = new List<Col_InsumoLaboral>();
                        foreach (dynamic item in insumosJson)
                        {
                            Col_InsumoLaboral _insumoLaboral = new Col_InsumoLaboral();
                            _insumoLaboral.Nombre = item.Nombre;
                            insumoLaborales.Add(_insumoLaboral);
                        }
                    }

                    int result = laboralActualizar == 0 ? await service.GuardarLaboborales(_laboral, insumoLaborales, personaId)
                        : await service.ActualizarLaboborales(_laboral, insumoLaborales, laboralActualizar);
                    return Json(result);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public async Task<IActionResult> CargarRoles()
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                bool leer = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Leer")).Any();
                if (leer)
                {
                    List<Col_Roles> roles = await service.MostrarRoles();
                    return Json(new { result = "ok", data = roles });
                }
                else
                {
                    return NotFound();
                }
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarAfiliacion(string afiliaciones, int rol, int laboralId, string persona, int afiliacioneActualizar)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                bool crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                bool Actualizar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Actualizar")).Any();
                if (crear || (Actualizar && afiliacioneActualizar != 0))
                {
                    dynamic afiliacionesJson = JsonConvert.DeserializeObject(afiliaciones);
                    dynamic personaJson = JsonConvert.DeserializeObject(persona);

                    Col_Personas _persona = new Col_Personas();
                    _persona.TipoDocumento = personaJson.TipoDocumento;
                    _persona.PrimerNombre = personaJson.PrimerNombre;
                    _persona.PrimerApellido = personaJson.PrimerApellido;
                    _persona.NumeroDocumento = personaJson.NumeroDocumento;

                    List<Col_Afiliacion> _afiliaciones = new List<Col_Afiliacion>();
                    foreach (dynamic item in afiliacionesJson)
                    {
                        Col_Afiliacion afiliacion = new Col_Afiliacion();
                        afiliacion.FechaAfiliacion = Convert.ToDateTime(item.FechaAfiliacion.ToString());
                        afiliacion.NombreEntidad = item.NombreEntidad;
                        afiliacion.TipoEntidad = item.TipoEntidad;
                        _afiliaciones.Add(afiliacion);
                    }
                    ApiCallResult result = afiliacioneActualizar == 0 ? await service.GuardarAfiliacion(_afiliaciones, rol, laboralId, _persona)
                        : await service.ActualizarAfiliciacion(_afiliaciones, afiliacioneActualizar);
                    return Json(result);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");

        }

        [HttpGet]
        public async Task<IActionResult> MostrarPendientes(char progreso, int idPersona)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                bool crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    Progresos result = await service.MostrarPendientes(progreso, idPersona);
                    return Json(result);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarEmpleado(int personaId, bool op)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                bool eliminar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Eliminar")).Any();
                if (eliminar)
                {
                    ApiCallResult result = await service.EliminarEmpleado(personaId, op);
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

        [HttpPut]
        public async Task<IActionResult> ActivarEmpleado(int personaId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                bool actualizar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Actualizar")).Any();
                if (actualizar)
                {
                    ApiCallResult result = await service.ActivarPerfil(personaId);
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
