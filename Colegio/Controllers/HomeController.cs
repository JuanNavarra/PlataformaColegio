using System;
using Microsoft.AspNetCore.Mvc;

namespace Colegio.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return Redirect("~/Login/Authentication");
        }
    }
}
