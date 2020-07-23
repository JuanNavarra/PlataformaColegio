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
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                    var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                    if (crear)
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
                        persona.SegundoApellido = personalJson.PrimerApellido;

                        List<Col_InfoAcademica> infoAcademicas = new List<Col_InfoAcademica>();
                        foreach (var item in academicoJson)
                        {
                            Col_InfoAcademica infoAcademica = new Col_InfoAcademica();
                            infoAcademica.FechaGradua = Convert.ToDateTime(item.FechaGradua.ToString());
                            infoAcademica.NivelFormacion = item.NivelFormacion;
                            infoAcademica.NombreIns = item.NombreIns;
                            infoAcademica.TituloObtenido = item.TituloObtenido;
                            infoAcademicas.Add(infoAcademica);
                        }

                        var result = await service.GuardarPersonales(persona, infoAcademicas);
                        return Json(result);
                    }
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Login");
            }
            #region catch
            catch (DbEntityValidationException e)
            {
                string err = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        err += ve.ErrorMessage;
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return null;
            }

            catch (Exception e)
            {
                string err = "";
                if (e.InnerException != null)
                {
                    if (e.InnerException.Message != null)
                    {
                        err = (e.InnerException.Message);
                        if (e.InnerException.InnerException != null)
                        {
                            err += e.InnerException.InnerException.Message;
                        }
                    }
                }
                else
                {
                    err = (e.Message);
                }
                return null;
            }
            #endregion
        }

        [HttpPost]
        public async Task<IActionResult> GuardarExperiencia(string experiencia, int personaId)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                    var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                    if (crear)
                    {
                        dynamic experienciaJson = JsonConvert.DeserializeObject(experiencia);
                        List<Col_Experiencia> experiencias = new List<Col_Experiencia>();

                        foreach (var item in experienciaJson)
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
                        var result = await service.GuardarExperiencia(experiencias, personaId);
                        return Json(result);
                    }
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Login");
            }
            #region catch
            catch (DbEntityValidationException e)
            {
                string err = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        err += ve.ErrorMessage;
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return null;
            }

            catch (Exception e)
            {
                string err = "";
                if (e.InnerException != null)
                {
                    if (e.InnerException.Message != null)
                    {
                        err = (e.InnerException.Message);
                        if (e.InnerException.InnerException != null)
                        {
                            err += e.InnerException.InnerException.Message;
                        }
                    }
                }
                else
                {
                    err = (e.Message);
                }
                return null;
            }
            #endregion
        }
    }
}
