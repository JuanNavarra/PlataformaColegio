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
        private readonly IPerfilesService service;

        public PerfilesController(IPerfilesService perfilesService)
        {
            this.service = perfilesService;
        }

        private PermisosCRUD Permisos(string modulo)
        {
            PermisosCRUD permiso = new PermisosCRUD();
            var permisos = User.Claims
                        .Where(w => w.Type.Equals(modulo) && w.Value.Contains("Maestro Administrativo")).ToList();
            permiso.PSMAPB = permisos.Where(w => w.Value.Contains("Perfiles")).Any();
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
                    var registros = await service.MostrarAutorizaciones();
                    ViewBag.Registros = registros;

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
        public async Task<IActionResult> GuardarCambios(string modulo, string subModulo, string rol, string descripcion, int idRol)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                var actualizar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Actualizar")).Any();
                if ((crear && idRol == 0) || (actualizar && idRol != 0))
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

                    ApiCallResult result = idRol == 0 ? await service.GuardarAutorizaciones(modulos, subModulos, rol, descripcion) :
                        await service.ActualizarAutorizaciones(modulos, subModulos, rol, descripcion, idRol);
                    return Json(new { result });
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
        public async Task<IActionResult> CargarModulos()
        {
            if (User.Identity.IsAuthenticated)
            {
                var modulos = await service.CargarModulos();
                return Json(new { result = "ok", data = modulos });
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> CargarSubModulos(int[] modulos)
        {
            if (User.Identity.IsAuthenticated)
            {
                var subModulos = await service.CargarSubModulos(modulos);
                return Json(new { result = "ok", data = subModulos });
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public async Task<IActionResult> MostrarDetallePerfil(int rolId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var leer = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Leer")).Any();
                if (leer)
                {
                    var perfil = await service.MostrarDetallePerfil(rolId);
                    return Json(new { result = "ok", data = perfil });
                }
                else
                {
                    return NotFound();
                }
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarPerfiles(int rolId, bool op)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var eliminar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Eliminar")).Any();
                if (eliminar)
                {
                    var result = await service.EliminarPerfiles(rolId, op);
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
        public async Task<IActionResult> CargarDatosActualizar(int rolId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var actualizar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Actualizar")).Any();
                if (actualizar)
                {
                    var data = await service.CargaDatosActualizar(rolId);
                    return Json(new { result = "ok", data });
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
        public async Task<IActionResult> ActivarPerfil(int rolId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var actualizar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Actualizar")).Any();
                if (actualizar)
                {
                    var result = await service.ActivarPerfil(rolId);
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
