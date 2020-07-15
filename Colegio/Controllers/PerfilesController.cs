using Colegio.Models;
using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Controllers
{
    public class PerfilesController : Controller
    {
        private readonly IPerfilesService perfilesService;

        public PerfilesController(IPerfilesService perfilesService)
        {
            this.perfilesService = perfilesService;
        }

        private bool GenerarPermisos(string subModulo)
        {
            var permisos = User.Claims
                .Where(w => w.Type.Equals("PermisoSubModulo") && w.Value.StartsWith("Maestro Administrativo")
                && w.Value.Contains(subModulo)).Any();
            return permisos;
        }

        private bool GenerarPermisosGlobales()
        {
            var permisos = User.Claims
                .Where(w => w.Type.Equals("PermisoModulo") && w.Value.Contains("Maestro Administrativo"))
                .Any();
            return permisos;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (GenerarPermisos("Perfiles") || (!GenerarPermisos("Perfiles") && GenerarPermisosGlobales()))
                {
                    var registros = await perfilesService.MostrarAutorizaciones();
                    ViewBag.Registros = registros;
                    return View();
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios(string modulo, string subModulo, string rol, string descripcion, int idRol)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    dynamic moduloJson = JsonConvert.DeserializeObject(modulo);
                    dynamic subModuloJson = JsonConvert.DeserializeObject(subModulo);
                    List<Col_RolModulos> modulos = new List<Col_RolModulos>();
                    List<Col_SubModuloModulo> subModulos = new List<Col_SubModuloModulo>();
                    foreach (var item in moduloJson)
                    {
                        Col_RolModulos _modulo = new Col_RolModulos();
                        _modulo.ModuloId = item.ModuloId;
                        _modulo.PermisosCrud = item.PermisosCrud;
                        modulos.Add(_modulo);
                    }
                    if (subModuloJson.Count > 0)
                    {
                        foreach (var item in subModuloJson)
                        {
                            Col_SubModuloModulo _subModulos = new Col_SubModuloModulo();
                            _subModulos.SubModuloId = item.SubModuloId;
                            _subModulos.ModuloId = item.ModuloId;
                            _subModulos.PermisosCrud = item.PermisosCrud;
                            subModulos.Add(_subModulos);
                        }
                    }

                    ApiCallResult result = idRol == 0 ? await perfilesService.GuardarAutorizaciones(modulos, subModulos, rol, descripcion) :
                        await perfilesService.ActualizarAutorizaciones(modulos, subModulos, rol, descripcion, idRol);
                    return Json(new { result });
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
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CargarModulos()
        {
            if (User.Identity.IsAuthenticated)
            {
                var modulos = await perfilesService.CargarModulos();
                return Json(new { result = "ok", data = modulos });
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> CargarSubModulos(int[] modulos)
        {
            if (User.Identity.IsAuthenticated)
            {
                var subModulos = await perfilesService.CargarSubModulos(modulos);
                return Json(new { result = "ok", data = subModulos });
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public async Task<IActionResult> MostrarDetallePerfil(int rolId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var perfil = await perfilesService.MostrarDetallePerfil(rolId);
                return Json(new { result = "ok", data = perfil });
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarPerfiles(int rolId, bool op)
        {
            if (User.Identity.IsAuthenticated)
            {
                var result = await perfilesService.EliminarPerfiles(rolId, op);
                return Json(result);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CargarDatosActualizar(int rolId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var data = await perfilesService.CargaDatosActualizar(rolId);
                return Json(new { result = "ok", data });
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}
