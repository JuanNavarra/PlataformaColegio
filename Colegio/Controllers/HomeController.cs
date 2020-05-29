using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
                HttpContext.Session.SetString("JWToken", userToken);
                bool islogin = User.Identity.IsAuthenticated;
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
