using System;
using Microsoft.AspNetCore.Mvc;

namespace Colegio.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return View();
                }
                return Redirect("~/Login/Authentication");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
