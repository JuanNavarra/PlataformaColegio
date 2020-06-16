using System;
using Microsoft.AspNetCore.Mvc;
using Colegio.Models;
using Microsoft.AspNetCore.Http;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Colegio.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly ITokenProvider tokenProvider;

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
                return View();
            }
            return Redirect("~/Home/");
        }

        /// <summary>
        /// Verifica si los datos traidos por el servicio LoginUser son correctos
        /// </summary>
        /// <param name="user">Objeto tipo Col_Usuarios que tiene el usuario y la contraseña</param>
        /// <returns>Home de la aplicación si las credenciales son correctas, de lo contrario
        /// retorna la pagina de autenticación
        /// </returns>
        [HttpPost]
        public IActionResult LoginUser(Col_Usuarios user)
        {
            try
            {
                var userToken = tokenProvider.LoginUser(user.Usuario.Trim(), user.Contrasena);
                if (userToken != null)
                {
                    //Save token in session object
                    HttpContext.Session.SetString("JWToken", userToken.AuthenticationType);
                    return Redirect("~/Home/");
                }
                return Redirect("~/Login/Authentication");
            }
            catch (Exception)
            {
                throw;
            }
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
            return Redirect("~/Login/Authentication");
        }
    }
}
