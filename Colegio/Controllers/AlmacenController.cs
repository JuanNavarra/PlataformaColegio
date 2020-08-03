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
    public class AlmacenController : Controller
    {
        private readonly IAlmacen service;

        public AlmacenController(IAlmacen almacenService)
        {
            this.service = almacenService;
        }

        private PermisosCRUD Permisos(string modulo)
        {
            PermisosCRUD permiso = new PermisosCRUD();
            var permisos = User.Claims
                        .Where(w => w.Type.Equals(modulo) && w.Value.Contains("Maestro Administrativo")).ToList();
            permiso.PSMAPB = permisos.Where(w => w.Value.Contains("Almacen")).Any();
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
        public async Task<IActionResult> GuardarSuministros(string suministros)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    dynamic suministroJson = JsonConvert.DeserializeObject(suministros);
                    Col_Suministros _suministro = new Col_Suministros();
                    _suministro.Descripcion = suministroJson.Descripcion;
                    _suministro.Nombre = suministroJson.Nombre;
                    _suministro.Stock = suministroJson.Stock;
                    _suministro.Talla = suministroJson.Talla;
                    _suministro.Linea = suministroJson.Linea;
                    _suministro.TipoSuministro = suministroJson.TipoSuministro;
                    var result = await service.GuardarSuministros(_suministro);
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
        public async Task<IActionResult> BuscarEmpleado(string documento)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    var empleado = await service.BuscarEmpleado(documento);
                    return Json(new { data = empleado });
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
        public async Task<IActionResult> MostrarSuministros()
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    var suministros = await service.MostrarSuministros();
                    return Json(new { data = suministros });
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
        public async Task<IActionResult> PrestarInsumos(string prestamos, string documento)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    dynamic prestamosJson = JsonConvert.DeserializeObject(prestamos);
                    List<Col_Prestamos> _prestamos = new List<Col_Prestamos>();
                    foreach (var item in prestamosJson)
                    {
                        Col_Prestamos _prestamo = new Col_Prestamos();
                        _prestamo.Cantidad = item.Cantidad;
                        _prestamo.FechaPrestamo = Convert.ToDateTime(item.FechaPrestamo.ToString());
                        _prestamo.Motivo = item.Motivo;
                        _prestamo.SuministroId = item.SuministroId;
                        _prestamos.Add(_prestamo);
                    }
                    var data = await service.PrestarInsumos(_prestamos, documento);
                    return Json(data);
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
        public async Task<IActionResult> BuscarDevolver(string documento)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    var insumos = await service.BuscarDevolucion(documento);
                    return Json(insumos);
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
        public async Task<IActionResult> DevolverInsumos(string devoluciones)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                if (crear)
                {
                    dynamic devolucionJson = JsonConvert.DeserializeObject(devoluciones);
                    List<Devoluciones> _devoluciones = new List<Devoluciones>();
                    foreach (var item in devolucionJson)
                    {
                        Devoluciones _devolucion = new Devoluciones();
                        _devolucion.IdPersona = item.idPersona;
                        _devolucion.IdPrestamo = item.idPrestamo;
                        _devolucion.Devolver = item.incremento;
                        _devolucion.SuministroId = item.suministroId;
                        _devolucion.Cantidad = item.cantidadActual;
                        _devoluciones.Add(_devolucion);
                    }
                    var insumos = await service.DevolverInsumos(_devoluciones);
                    return Json(insumos);
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
        public async Task<IActionResult> MostrarInfoSuministros()
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var crear = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Crear")).Any();
                var leer = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Leer")).Any();
                if (crear && leer)
                {
                    var insumos = await service.ListarSuministros();
                    return Json(insumos);
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
        public async Task<IActionResult> VaciarStock(int suministroId, int stock)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var eliminar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Eliminar")).Any();
                if (eliminar)
                {
                    var insumos = await service.VaciarStock(suministroId, stock);
                    return Json(insumos);
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

        [HttpDelete]
        public async Task<IActionResult> EliminarSuministros(int suministroId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string permiso = Permisos("PermisoSubModulo").PSMAPB ? "PermisoSubModulo" : "PermisoModulo";
                var eliminar = Permisos(permiso).PMMAPL.Where(w => w.Value.Contains("Eliminar")).Any();
                if (eliminar)
                {
                    var insumos = await service.EliminarSuministros(suministroId);
                    return Json(insumos);
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
