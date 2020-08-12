using System;
using Microsoft.AspNetCore.Mvc;
using Colegio.Models;
using Microsoft.AspNetCore.Http;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Colegio.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly ITokenProvider tokenProvider;
        private static bool sesion = true;

        public LoginController(ITokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
        }

        [HttpGet]
        [Route("Authentication")]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.Bool = sesion;
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Verifica si los datos traidos por el servicio LoginUser son correctos
        /// </summary>
        /// <param name="user">Objeto tipo Col_Usuarios que tiene el usuario y la contraseña</param>
        /// <returns>Home de la aplicación si las credenciales son correctas, de lo contrario
        /// retorna la pagina de autenticación
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> LoginUser(Col_Usuarios user)
        {
            try
            {
                ClaimsIdentity userToken = await tokenProvider.LoginUser(user.Usuario.Trim(), user.Contrasena);
                if (userToken != null)
                {
                    //Save token in session object
                    HttpContext.Session.SetString("JWToken", userToken.AuthenticationType);
                    return Redirect("~/Home/");
                }
                sesion = false;
                return RedirectToAction("Index", "Login");
            }
            #region catch
            catch (DbEntityValidationException e)
            {
                string err = "";
                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (DbValidationError ve in eve.ValidationErrors)
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

        /// <summary>
        /// Destruye la sesion existente, solo se invocara si hay una sesión abierta
        /// </summary>
        /// <returns>Redirige al login de la aplicación</returns>
        [HttpGet]
        [Authorize]
        [Route("LogOut")]
        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
