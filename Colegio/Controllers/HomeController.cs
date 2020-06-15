using System;
using Microsoft.AspNetCore.Mvc;
using Colegio.Models;
using Colegio.Services;
using Microsoft.AspNetCore.Http;

namespace Colegio.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginUser(User user)
        {
            TokenProvider _tokenProvider = new TokenProvider();
            var userToken = _tokenProvider.LoginUser(user.UserId.Trim(), user.Password);
            if (userToken != null)
            {
                //Save token in session object
                HttpContext.Session.SetString("JWToken", userToken.AuthenticationType);
                if (userToken.IsAuthenticated)
                {
                    Console.WriteLine("sas");
                }
            }
            return Redirect("~/Home/Index");
        }

        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Home/Index");
        }
    }
}
