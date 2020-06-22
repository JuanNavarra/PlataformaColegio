using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colegio.Models;
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

        public async Task<IActionResult> GuardarAutorizaciones(string modulo, string subModulo, string rol, bool swRol)
        {
            try
            {
                var moduloComverted = JsonConvert.DeserializeObject<Col_Modulos>(modulo);
                dynamic subModulosJson = JsonConvert.DeserializeObject(subModulo);
                List<Col_SubModulos> subModulos = new List<Col_SubModulos>();
                if (moduloComverted.EsPadre)
                {
                    foreach (var item in subModulosJson)
                    {
                        Col_SubModulos _subModulo = new Col_SubModulos();
                        _subModulo.SubModuloId = item.SubModuloId;
                        _subModulo.ModuloId = 0;
                        subModulos.Add(_subModulo);
                    }
                }
                var modulos = await perfilesService.GuardarAutorizaciones(moduloComverted, subModulos, rol, swRol);
                return Json(new { result = modulos.Status, title = modulos.Title, message = modulos.Message });
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
    }
}
