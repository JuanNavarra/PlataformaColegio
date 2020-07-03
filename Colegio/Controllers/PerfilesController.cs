using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colegio.Models;
using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return Redirect("~/Login/Authentication");
        }

        public async Task<IActionResult> GuardarAutorizaciones(string modulo, string subModulo, string rol, string autorizacion, string descripcion)
        {
            try
            {
                dynamic moduloJson = JsonConvert.DeserializeObject(modulo);
                dynamic subModuloJson = JsonConvert.DeserializeObject(subModulo);
                List<Col_Modulos> modulos = new List<Col_Modulos>();
                List<Col_SubModulos> subModulos = new List<Col_SubModulos>();
                foreach (var item in moduloJson)
                {
                    Col_Modulos _modulo = new Col_Modulos();
                    _modulo.ModuloId = item.ModuloId;
                    modulos.Add(_modulo);
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

                var result = await perfilesService.GuardarAutorizaciones(modulos,subModulos,rol,autorizacion,descripcion);
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

        public async Task<IActionResult> CargarRoles()
        {
            var roles = await perfilesService.CargarRol();
            return Json(new { result = "ok", roles });
        }

        public async Task<IActionResult> CargarModulos()
        {
            var modulos = await perfilesService.CargarModulos();
            return Json(new { result = "ok", data = modulos });
        }

        public async Task<IActionResult> CargarSubModulos(int[] modulos)
        {
            var subModulos = await perfilesService.CargarSubModulos(modulos);
            return Json(new { result = "ok", data = subModulos });
        }
    }
}
