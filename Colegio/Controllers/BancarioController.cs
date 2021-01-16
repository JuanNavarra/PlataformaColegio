using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Colegio.Controllers
{
    public class BancarioController : Controller
    {

        private readonly IBancario service;

        public BancarioController(IBancario bancario)
        {
            this.service = bancario;
        }

        private PermisosCRUD Permisos(string modulo)
        {
            PermisosCRUD permiso = new PermisosCRUD();
            List<Claim> permisos = User.Claims
                        .Where(w => w.Type.Equals(modulo) && w.Value.Contains("Maestro Administrativo")).ToList();
            permiso.PSMAPB = permisos.Where(w => w.Value.Contains("Bancario")).Any();
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
                    List<Claim> permisos = Permisos(modulo).PMMAPL;
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
        public async Task<IActionResult> GuardarCuenta(string nombre, string titular, string numero)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                bool crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    ApiCallResult result = await service.GuardarCuenta(nombre, titular, numero);
                    return Json(result);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }
    }
}
