using Colegio.Models;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var registros = await perfilesService.MostrarAutorizaciones();
                ViewBag.Registros = registros;
                return View();
            }
            return Redirect("~/Login/Authentication");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarAutorizaciones(string modulo, string subModulo, string rol, string crud, string descripcion)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    dynamic moduloJson = JsonConvert.DeserializeObject(modulo);
                    dynamic subModuloJson = JsonConvert.DeserializeObject(subModulo);
                    dynamic crudJson = JsonConvert.DeserializeObject(crud);
                    List<Col_Modulos> modulos = new List<Col_Modulos>();
                    List<Col_SubModulos> subModulos = new List<Col_SubModulos>();
                    List<Col_PermisosCrud> permisos = new List<Col_PermisosCrud>();
                    foreach (var item in moduloJson)
                    {
                        Col_Modulos _modulo = new Col_Modulos();
                        _modulo.ModuloId = item.ModuloId;
                        modulos.Add(_modulo);
                    }
                    foreach (var item in crudJson)
                    {
                        Col_PermisosCrud _permiso = new Col_PermisosCrud();
                        _permiso.PermisoId = item.PermisoId;
                        permisos.Add(_permiso);
                    }
                    if (subModuloJson.Count > 0)
                    {
                        foreach (var item in subModuloJson)
                        {
                            Col_SubModulos _subModulos = new Col_SubModulos();
                            _subModulos.SubModuloId = item.SubModuloId;
                            _subModulos.ModuloId = item.ModuloId;
                            subModulos.Add(_subModulos);
                        }
                    }

                    var result = await perfilesService.GuardarAutorizaciones(modulos, subModulos, rol, permisos, descripcion);
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
        public async Task<IActionResult> CargarRoles()
        {
            var roles = await perfilesService.CargarRol();
            return Json(new { result = "ok", roles });
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
        public async Task<IActionResult> CargarPermisosCRUD()
        {
            if (User.Identity.IsAuthenticated)
            {
                var crud = await perfilesService.CargarPermisosCRUD();
                return Json(new { result = "ok", data = crud });
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
        public  async Task<IActionResult> EliminarPerfiles(int rolId, bool op)
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
