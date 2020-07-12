using Colegio.Models;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
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

        private List<Claim> GenerarPermisos()
        {
            var permisos = User.Claims
                .Where(w => w.Type.Equals("PermisoSubModulo") && w.Value.Contains("Maestro Administrativo"))
                .ToList();
            return permisos;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (GenerarPermisos().Where(w => w.Value.Contains("Perfiles")).Any())
                {
                    var registros = await perfilesService.MostrarAutorizaciones();
                    ViewBag.Registros = registros;
                    return View();
                }
            }
            return Redirect("~/Login/Authentication");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarAutorizaciones(string modulo, string subModulo, string rol, string descripcion)
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

                    var result = await perfilesService.GuardarAutorizaciones(modulos, subModulos, rol, descripcion);
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
                return Redirect("~/Login/Authentication");
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
            return Redirect("~/Login/Authentication");
        }

        [HttpPost]
        public async Task<IActionResult> CargarSubModulos(int[] modulos)
        {
            if (User.Identity.IsAuthenticated)
            {
                var subModulos = await perfilesService.CargarSubModulos(modulos);
                return Json(new { result = "ok", data = subModulos });
            }
            return Redirect("~/Login/Authentication");
        }

        [HttpGet]
        public async Task<IActionResult> MostrarDetallePerfil(int rolId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var perfil = await perfilesService.MostrarDetallePerfil(rolId);
                return Json(new { result = "ok", data = perfil });
            }
            return Redirect("~/Login/Authentication");
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
                return Redirect("~/Login/Authentication");
            }
        }
    }
}
