using System;
using Microsoft.AspNetCore.Mvc;
using Colegio.Models;
using Microsoft.AspNetCore.Http;
using Colegio.Services.Interfaces;

namespace Colegio.Controllers
{
    public class LoginController : Controller
    {
        private readonly ITokenProvider tokenProvider;
        public LoginController(ITokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginUser(Col_Usuarios user)
        {
            try
            {
                var userToken = tokenProvider.LoginUser(user.Usuario.Trim(), user.Contrasena);
                if (userToken != null)
                {
                    //Save token in session object
                    HttpContext.Session.SetString("JWToken", userToken.AuthenticationType); 
                    return Redirect("~/Home/Index");
                }
                return Redirect("~/Login/Index");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Login/Index");
        }
    }
}
